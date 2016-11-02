using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Vistian.Reactive.Validation;

namespace Vistian.Reactive.UnitTests.Validation
{
    /// <summary>
    /// Simple test class allowing us to easily change the state of a IValidationComponent
    /// </summary>
    public class TextValidationComponent:IValidationComponent
    {
        public ValidationText Text { get { return _state.Value.Text; } }
        public bool IsValid { get { return _state.Value.IsValid; } } 
        public IObservable<ValidationState> ValidationStatusChange => _state.AsObservable();

        private readonly BehaviorSubject<ValidationState> _state;

        public TextValidationComponent()
        {
            _state = new BehaviorSubject<ValidationState>(new ValidationState(true,"component#1",this) );
        }

        public void PushState(ValidationState state)
        {
            _state.OnNext(state);
        }
    }
}