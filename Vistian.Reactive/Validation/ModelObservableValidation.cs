using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// More generic observable for determination of validity
    /// </summary>
    /// <remarks>
    /// We probably need a more 'complex' one, where the params of the validation block are 
    /// passed through ?
    /// Also, what about access to the view model to output the error message ?</remarks>
    public class ModelObservableValidation<TViewModel> : ReactiveObject, IValidationComponent, IDisposable
    {
        private bool _isValid;

        // the underlying connected observable for the validation change which is published
        private readonly IConnectableObservable<ValidationState> _validityConnectedObservable;

        private readonly ReplaySubject<ValidationState> _lastValidationStateSubject = new ReplaySubject<ValidationState>(1);

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isActive;

        private ValidationText _text;


        public ModelObservableValidation(TViewModel model, Func<TViewModel, IObservable<bool>> validityObservable, Func<TViewModel, bool, string> messageFunc) : this(model, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        public ModelObservableValidation(TViewModel model, Func<TViewModel, IObservable<bool>> validityObservable, Func<TViewModel, bool, ValidationText> messageFunc)
        {
            _disposables.Add(_lastValidationStateSubject.Do(s =>
            {
                _isValid = s.IsValid;
                _text = s.Text;
            }).Subscribe());

            _validityConnectedObservable = Observable.Defer(() => validityObservable(model)).
                                            Select(v => new ValidationState(v, messageFunc(model, v), this)).
                                            Multicast(_lastValidationStateSubject);
        }

        private void Activate()
        {
            if (!_isActive)
            {
                _isActive = true;
                _disposables.Add(_validityConnectedObservable.Connect());
            }
        }

        /// <summary>
        /// Get the current validation text
        /// </summary>
        public ValidationText Text
        {
            get
            {
                Activate();
                return _text;
            }
        }

        /// <summary>
        /// Get the current state
        /// </summary>
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid;
            }
        }

        /// <summary>
        /// Get the observable for <see cref="ValidationState"/> changes
        /// </summary>
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validityConnectedObservable;
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
