﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// The overall context for a view model underwhich validation takes place.
    /// </summary>
    /// <remarks>
    /// Contains all of the <see cref="IValidationComponent"/> instances applicable to the view model.</remarks>
    public class ValidationContext : ReactiveObject, IDisposable, IValidationComponent
    {
        /// <summary>
        /// Subject for validatity of the context.
        /// </summary>
        private readonly ReplaySubject<bool> _validSubject = new ReplaySubject<bool>(1);

        /// <summary>
        /// An observable for the Valid state
        /// </summary>
        public IObservable<bool> Valid
        {
            get
            {
                Activate();

                return _validSubject.AsObservable();
            }
        }

        /// <summary>
        /// Backing field for the current validation state
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        /// <summary>
        /// Get whether currently valid or not
        /// </summary>
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid.Value;
            }
        }

        private readonly ReplaySubject<ValidationState> _validationStatusChange = new ReplaySubject<ValidationState>(1);

        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validationStatusChange.AsObservable();
            }
        }

        /// <summary>
        /// The list of current validations
        /// </summary>
        private readonly ReactiveList<IValidationComponent> _validations = new ReactiveList<IValidationComponent>();

        /// <summary>
        /// Get the list of validations
        /// </summary>
        public IReadOnlyReactiveList<IValidationComponent> Validations => _validations;

        /// <summary>
        /// Backing field for the validation summary
        /// </summary>
        private readonly ObservableAsPropertyHelper<ValidationText> _validationText;

        /// <summary>
        /// Get the current validation summary.
        /// </summary>
        public ValidationText Text
        {
            get
            {
                Activate();
                return _validationText.Value;
            }
        }

        private void Activate()
        {
            if (!_isActive)
            {
                _isActive = true;

                _disposables.Add(_validationConnectable.Connect());
            }
        }

        /// <summary>
        /// What needs to be disposed off
        /// </summary>
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IConnectableObservable<bool> _validationConnectable;

        private bool _isActive;

        /// <summary>
        /// Create the context
        /// </summary>
        public ValidationContext()
        {
            // publish the current validation state 
            _disposables.Add(_validSubject.StartWith(true).ToProperty(this, m => m.IsValid, out _isValid));

            // when a change occurs in the validation state, publish the updated validation text
            _disposables.Add(_validSubject.StartWith(true).Select(v => BuildText())
                .ToProperty(this, m => m.Text, out _validationText, new ValidationText()));

            //publish the current validation state
            _disposables.Add(_validSubject.
                                Select(v => new ValidationState(IsValid, BuildText(), this)).
                                Do(vc => _validationStatusChange.OnNext(vc)).
                                Subscribe());

            // observe the defined validations and whenever there is a change publish the current validation state.
            _validationConnectable = _validations.CountChanged.
                                            StartWith(0).
                                            Select(_ => _validations.Select(v => v.ValidationStatusChange).Merge().Select((o) => Unit.Default).StartWith(Unit.Default)).
                                            Switch().
                                            Select(_ => GetIsValid()).
                                            Multicast(_validSubject);
        }

        /// <summary>
        /// Add a <see cref="IValidationComponent"/> to those being monitored for validation state
        /// </summary>
        /// <param name="validation"></param>
        public void Add(IValidationComponent validation)
        {
            _validations.Add(validation);
        }

        public bool GetIsValid()
        {
            return _validations.Count == 0 || _validations.All(v => v.IsValid);
        }

        /// <summary>
        /// Build a list of the validation text for each invalid component
        /// </summary>
        /// <returns></returns>

        private ValidationText BuildText()
        {
            return new ValidationText(_validations.Where(p => !p.IsValid).Select(p => p.Text));
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
