using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// Base class for items which are used to build a <see cref="ValidationContext"/>
    /// </summary>
    public abstract class BasePropertyValidation<TViewModel> : ReactiveObject, IDisposable, IValidationComponent
    {
        /// <summary>
        /// Our current validity state
        /// </summary>
        private bool _isValid;


        private ValidationText _text;

        /// <summary>
        /// The current valid state
        /// </summary>
        private readonly ReplaySubject<bool> _isValidSubject = new ReplaySubject<bool>(1);

        private bool _isConnected;

        /// <summary>
        /// The list of property names this validator
        /// </summary>
        private readonly HashSet<string> _propertyNames = new HashSet<string>();

        /// <summary>
        /// The items to be disposed.
        /// </summary>
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The connected observable to kick off seeing <see cref="ValidationStatusChange"/>
        /// </summary>
        private IConnectableObservable<ValidationState> _connectedChange;

        protected BasePropertyValidation()
        {
            // subscribe to the valid suject so we can asign the validity
            _disposables.Add(_isValidSubject.Subscribe(v => _isValid = v));
        }

        /// <summary>
        /// Determine if a property name is actually contained within this
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <param name="exclusively"></param>
        /// <returns></returns>
        public bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> property, bool exclusively = false)
        {
            var propertyName = property.Body.ToString();

            return exclusively ? _propertyNames.Contains(propertyName) && _propertyNames.Count == 1 : _propertyNames.Contains(propertyName);
        }

        /// <summary>
        /// Get the total number of properties referenced.
        /// </summary>
        public int PropertyCount => _propertyNames.Count;

        /// <summary>
        /// Add a property to the list of this which this validation is associated with.
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
        {
            var propertyName = property.Body.ToString();

            _propertyNames.Add(propertyName);
        }

        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid;
            }
        }

        /// <summary>
        /// The public mechanism indicating that the validation state has changed.
        /// </summary>
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();

                return _connectedChange;
            }
        }

        /// <summary>
        /// Get the validation change observable, implemented by concrete classes.
        /// </summary>
        /// <returns></returns>
        protected abstract IObservable<ValidationState> GetValidationChangeObservable();

        public ValidationText Text
        {
            get
            {
                Activate();

                return _text;
            }
        }


        private void Activate()
        {
            if (!_isConnected)
            {
                _connectedChange = GetValidationChangeObservable().Do(v =>
                {
                    _isValid = v.IsValid;
                    _text = v.Text;
                }).Replay(1);

                _disposables.Add(_connectedChange.Connect());

                _isConnected = true;
            }
        }

        public virtual void Dispose()
        {
            _disposables?.Dispose();
        }
    }


    /// <summary>
    /// Property validator for a single view model property.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TProperty1"></typeparam>
    public sealed class BasePropertyValidation<TViewModel, TProperty1> : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// The mechanism to determine if the property(s) is valid or not
        /// </summary>
        private Func<TProperty1, bool> IsValidFunc { get; }

        /// <summary>
        /// The message to be constructed.
        /// </summary>
        private readonly Func<TProperty1, bool, ValidationText> _message;

        /// <summary>
        /// The value calculated from the properties.
        /// </summary>
        private readonly ReplaySubject<TProperty1> _valueSubject = new ReplaySubject<TProperty1>(1);

        private readonly IConnectableObservable<TProperty1> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public BasePropertyValidation(TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty,
            Func<TProperty1, bool> isValidFunc, string message) : this(viewModel, viewModelProperty, isValidFunc, (p, v) => new ValidationText((v) ? string.Empty : message))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty,
            Func<TProperty1, bool> isValidFunc, Func<TProperty1, string> message) :
            this(viewModel, viewModelProperty, isValidFunc,
                (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty,
            Func<TProperty1, bool> isValidFunc,
            Func<TProperty1, bool, string> messageFunc) : this(viewModel, viewModelProperty, isValidFunc, (prop1, isValid) => new ValidationText(messageFunc(prop1, isValid)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty,
            Func<TProperty1, bool> isValidFunc,
            Func<TProperty1, bool, ValidationText> messageFunc) : base()
        {
            // now, we have a function, which, in this case uses the value of the view Model Property...
            //         
            IsValidFunc = isValidFunc;

            // record this property name
            this.AddProperty(viewModelProperty);

            // the function invoked
            _message = messageFunc;

            // our connected observable
            _valueConnectedObservable = viewModel.WhenAny(viewModelProperty, v => v.Value).
                                                  DistinctUntilChanged().
                                                  Multicast(_valueSubject);
        }

        /// <summary>
        /// Get the validation change observable.
        /// </summary>
        /// <returns></returns>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                Select(value => new ValidationState(IsValidFunc(value), this.GetMessage(value), this)).DistinctUntilChanged(new ValidationStateComparer());
        }

        private bool _isConnected;

        private void Activate()
        {
            if (!_isConnected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _isConnected = true;
            }
        }


        private ValidationText GetMessage(TProperty1 value)
        {
            // need something subtle to deal with validity having not actual message
            return _message(value, IsValidFunc(value));
        }
    }
}
