
    using System;
    using System.Linq.Expressions;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using ReactiveUI;

namespace Vistian.Reactive.Validation
{
	            	
    public sealed class BasePropertyValidation<TViewModel,TProperty1,TProperty2> : BasePropertyValidation<TViewModel>,IDisposable
    {
        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1,TProperty2> _lastValue;

        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1,TProperty2>> _valueSubject = new Subject<Tuple<TProperty1,TProperty2>>();

        /// <summary>
        /// The validation message factory
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2>,bool,ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1,TProperty2>> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Are we connected
        /// </summary>
        private bool _connected;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2>, bool> _isValidFunc;


        public BasePropertyValidation(TViewModel viewModel,

					Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
	
            Func<Tuple<TProperty1,TProperty2>, bool> isValidFunc,
			Func<Tuple<TProperty1,TProperty2>,string> message):

            this(viewModel,property1, property2,isValidFunc,
                (Tuple<TProperty1,TProperty2> p, bool v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,

			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
	
            Func<Tuple<TProperty1,TProperty2>, bool> isValidFunc,

            Func<Tuple<TProperty1,TProperty2>, bool, string> messageFunc):
			this(viewModel,property1, property2,isValidFunc,(parameters,isValid) => new ValidationText(messageFunc(parameters,isValid)))
        {            
        }


        public BasePropertyValidation(TViewModel viewModel,
			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Func<Tuple<TProperty1,TProperty2>,bool> validSelector,
		Func<Tuple<TProperty1,TProperty2>,bool,ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // add the properties used to our list
			AddProperty(property1);
			AddProperty(property2);
			
            // always record the last value seen
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2).DistinctUntilChanged().Multicast(_valueSubject);
        }

        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                    Select(value =>
                    {
                        var isValid = _isValidFunc(value);
                        return new ValidationState(isValid, this.GetMessage(value, isValid), this);
                    }).DistinctUntilChanged(new ValidationStateComparer());
        }


        private ValidationText GetMessage(Tuple<TProperty1,TProperty2> @params,bool isValid)
        {
            return _message(@params,isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _connected = true;
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
        	
    public sealed class BasePropertyValidation<TViewModel,TProperty1,TProperty2,TProperty3> : BasePropertyValidation<TViewModel>,IDisposable
    {
        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1,TProperty2,TProperty3> _lastValue;

        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1,TProperty2,TProperty3>> _valueSubject = new Subject<Tuple<TProperty1,TProperty2,TProperty3>>();

        /// <summary>
        /// The validation message factory
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3>,bool,ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1,TProperty2,TProperty3>> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Are we connected
        /// </summary>
        private bool _connected;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3>, bool> _isValidFunc;


        public BasePropertyValidation(TViewModel viewModel,

					Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3>, bool> isValidFunc,
			Func<Tuple<TProperty1,TProperty2,TProperty3>,string> message):

            this(viewModel,property1, property2, property3,isValidFunc,
                (Tuple<TProperty1,TProperty2,TProperty3> p, bool v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,

			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3>, bool> isValidFunc,

            Func<Tuple<TProperty1,TProperty2,TProperty3>, bool, string> messageFunc):
			this(viewModel,property1, property2, property3,isValidFunc,(parameters,isValid) => new ValidationText(messageFunc(parameters,isValid)))
        {            
        }


        public BasePropertyValidation(TViewModel viewModel,
			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Func<Tuple<TProperty1,TProperty2,TProperty3>,bool> validSelector,
		Func<Tuple<TProperty1,TProperty2,TProperty3>,bool,ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // add the properties used to our list
			AddProperty(property1);
			AddProperty(property2);
			AddProperty(property3);
			
            // always record the last value seen
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3).DistinctUntilChanged().Multicast(_valueSubject);
        }

        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                    Select(value =>
                    {
                        var isValid = _isValidFunc(value);
                        return new ValidationState(isValid, this.GetMessage(value, isValid), this);
                    }).DistinctUntilChanged(new ValidationStateComparer());
        }


        private ValidationText GetMessage(Tuple<TProperty1,TProperty2,TProperty3> @params,bool isValid)
        {
            return _message(@params,isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _connected = true;
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
        	
    public sealed class BasePropertyValidation<TViewModel,TProperty1,TProperty2,TProperty3,TProperty4> : BasePropertyValidation<TViewModel>,IDisposable
    {
        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1,TProperty2,TProperty3,TProperty4> _lastValue;

        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>> _valueSubject = new Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>>();

        /// <summary>
        /// The validation message factory
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>,bool,ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Are we connected
        /// </summary>
        private bool _connected;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>, bool> _isValidFunc;


        public BasePropertyValidation(TViewModel viewModel,

					Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>, bool> isValidFunc,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>,string> message):

            this(viewModel,property1, property2, property3, property4,isValidFunc,
                (Tuple<TProperty1,TProperty2,TProperty3,TProperty4> p, bool v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,

			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>, bool> isValidFunc,

            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>, bool, string> messageFunc):
			this(viewModel,property1, property2, property3, property4,isValidFunc,(parameters,isValid) => new ValidationText(messageFunc(parameters,isValid)))
        {            
        }


        public BasePropertyValidation(TViewModel viewModel,
			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>,bool> validSelector,
		Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4>,bool,ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // add the properties used to our list
			AddProperty(property1);
			AddProperty(property2);
			AddProperty(property3);
			AddProperty(property4);
			
            // always record the last value seen
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4).DistinctUntilChanged().Multicast(_valueSubject);
        }

        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                    Select(value =>
                    {
                        var isValid = _isValidFunc(value);
                        return new ValidationState(isValid, this.GetMessage(value, isValid), this);
                    }).DistinctUntilChanged(new ValidationStateComparer());
        }


        private ValidationText GetMessage(Tuple<TProperty1,TProperty2,TProperty3,TProperty4> @params,bool isValid)
        {
            return _message(@params,isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _connected = true;
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
        	
    public sealed class BasePropertyValidation<TViewModel,TProperty1,TProperty2,TProperty3,TProperty4,TProperty5> : BasePropertyValidation<TViewModel>,IDisposable
    {
        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5> _lastValue;

        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>> _valueSubject = new Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>>();

        /// <summary>
        /// The validation message factory
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>,bool,ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Are we connected
        /// </summary>
        private bool _connected;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>, bool> _isValidFunc;


        public BasePropertyValidation(TViewModel viewModel,

					Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>, bool> isValidFunc,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>,string> message):

            this(viewModel,property1, property2, property3, property4, property5,isValidFunc,
                (Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5> p, bool v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,

			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>, bool> isValidFunc,

            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>, bool, string> messageFunc):
			this(viewModel,property1, property2, property3, property4, property5,isValidFunc,(parameters,isValid) => new ValidationText(messageFunc(parameters,isValid)))
        {            
        }


        public BasePropertyValidation(TViewModel viewModel,
			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>,bool> validSelector,
		Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5>,bool,ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // add the properties used to our list
			AddProperty(property1);
			AddProperty(property2);
			AddProperty(property3);
			AddProperty(property4);
			AddProperty(property5);
			
            // always record the last value seen
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4, property5).DistinctUntilChanged().Multicast(_valueSubject);
        }

        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                    Select(value =>
                    {
                        var isValid = _isValidFunc(value);
                        return new ValidationState(isValid, this.GetMessage(value, isValid), this);
                    }).DistinctUntilChanged(new ValidationStateComparer());
        }


        private ValidationText GetMessage(Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5> @params,bool isValid)
        {
            return _message(@params,isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _connected = true;
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
        	
    public sealed class BasePropertyValidation<TViewModel,TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6> : BasePropertyValidation<TViewModel>,IDisposable
    {
        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6> _lastValue;

        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>> _valueSubject = new Subject<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>>();

        /// <summary>
        /// The validation message factory
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>,bool,ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>> _valueConnectedObservable;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Are we connected
        /// </summary>
        private bool _connected;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>, bool> _isValidFunc;


        public BasePropertyValidation(TViewModel viewModel,

					Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
			Expression<Func<TViewModel,TProperty6>> property6,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>, bool> isValidFunc,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>,string> message):

            this(viewModel,property1, property2, property3, property4, property5, property6,isValidFunc,
                (Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6> p, bool v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        public BasePropertyValidation(TViewModel viewModel,

			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
			Expression<Func<TViewModel,TProperty6>> property6,
	
            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>, bool> isValidFunc,

            Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>, bool, string> messageFunc):
			this(viewModel,property1, property2, property3, property4, property5, property6,isValidFunc,(parameters,isValid) => new ValidationText(messageFunc(parameters,isValid)))
        {            
        }


        public BasePropertyValidation(TViewModel viewModel,
			Expression<Func<TViewModel,TProperty1>> property1,
			Expression<Func<TViewModel,TProperty2>> property2,
			Expression<Func<TViewModel,TProperty3>> property3,
			Expression<Func<TViewModel,TProperty4>> property4,
			Expression<Func<TViewModel,TProperty5>> property5,
			Expression<Func<TViewModel,TProperty6>> property6,
			Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>,bool> validSelector,
		Func<Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6>,bool,ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // add the properties used to our list
			AddProperty(property1);
			AddProperty(property2);
			AddProperty(property3);
			AddProperty(property4);
			AddProperty(property5);
			AddProperty(property6);
			
            // always record the last value seen
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4, property5, property6).DistinctUntilChanged().Multicast(_valueSubject);
        }

        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.
                    Select(value =>
                    {
                        var isValid = _isValidFunc(value);
                        return new ValidationState(isValid, this.GetMessage(value, isValid), this);
                    }).DistinctUntilChanged(new ValidationStateComparer());
        }


        private ValidationText GetMessage(Tuple<TProperty1,TProperty2,TProperty3,TProperty4,TProperty5,TProperty6> @params,bool isValid)
        {
            return _message(@params,isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _disposables.Add(_valueConnectedObservable.Connect());

                _connected = true;
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
}