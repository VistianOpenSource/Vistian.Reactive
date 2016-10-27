using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// Encapsulation of a validation with bindable properties.
    /// </summary>
    public class ValidationHelper : ReactiveObject, IDisposable
    {
        private readonly IValidationComponent _validation;

        // how do we get this to be reactive though? we need to publish 
        // validation object
        private ObservableAsPropertyHelper<bool> _isValid;

        public bool IsValid => _isValid.Value;

        private ObservableAsPropertyHelper<ValidationText> _message;
        public ValidationText Message => _message.Value;

        public IObservable<ValidationState> ValidationChanged => _validation.ValidationStatusChange;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ValidationHelper(IValidationComponent validation)
        {
            _validation = validation;
            Setup();
        }

        private void Setup()
        {
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.IsValid).ToProperty(this, vm => vm.IsValid, out _isValid));
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.Text).ToProperty(this, vm => vm.Message, out _message));
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
