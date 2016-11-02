using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Vistian.Reactive.Validation;
using Xunit;

namespace Vistian.Reactive.UnitTests.Validation
{
    public class ValidationContextTests
    {
        private const string ComponentValidationText1 = "failed";
        private const string ComponentValidationText2 = "failed2";

        [Fact]
        public void EmptyContextTest()
        {
            var vc = new ValidationContext();

            Assert.True(vc.IsValid);
            Assert.Equal(0,vc.Validations.Count);
            Assert.Equal(String.Empty, vc.Text.ToSingleLine());
        }

        [Fact]
        public void CanAddComponentTest()
        {
            var vc = new ValidationContext();

            var component = new TextValidationComponent();
            vc.Add(component);

            Assert.True(vc.IsValid);
        }

        [Fact]
        public void ComponentChangesTriggersChangeTest()
        {
            var vc = new ValidationContext();

            var component = new TextValidationComponent();
            vc.Add(component);

            var stateChanges = new List<bool>();
            List<ValidationState> changes = new List<ValidationState>();

            vc.ValidationStatusChange.Subscribe(v => changes.Add(v));
            vc.Valid.Subscribe(v => stateChanges.Add(v));

            Assert.Equal(1,changes.Count);
            Assert.True(changes[0].IsValid);
            Assert.Equal(1,stateChanges.Count);

            var failedState = new ValidationState(false, ComponentValidationText1, component);
            component.PushState(failedState);

            Assert.Equal(2,changes.Count);
            Assert.Equal(2,stateChanges.Count);
            Assert.False(changes[1].IsValid);
            Assert.False(stateChanges[1]);
            Assert.Equal(ComponentValidationText1,changes[1].Text.ToSingleLine());
            Assert.Equal(ComponentValidationText1,vc.Text.ToSingleLine());
            Assert.False(vc.IsValid);
        }

        [Fact]
        public void MultipleComponentsCorrectStateTest()
        {
            var vc = new ValidationContext();

            var component1 = new TextValidationComponent();
            vc.Add(component1);

            var component2 = new TextValidationComponent();
            vc.Add(component2);

            var failedState1 = new ValidationState(false,ComponentValidationText1,component1);
            var successState1 = new ValidationState(true,string.Empty, component1);

            var failedState2 = new ValidationState(false, ComponentValidationText2, component2);
            var successState2 = new ValidationState(true, string.Empty, component2);

            component1.PushState(failedState1);

            Assert.False(vc.IsValid);

            component1.PushState(successState1);
            Assert.True(vc.IsValid);

            component1.PushState(failedState1);
            component2.PushState(failedState2);

            Assert.False(vc.IsValid);

            var vt = vc.Text;
            Assert.Equal(2,vt.Count);

            component2.PushState(successState2);

            Assert.False(vc.IsValid);
            Assert.Equal(1,vc.Text.Count);

            component1.PushState(successState1);

            Assert.True(vc.IsValid);
            Assert.Equal(0, vc.Text.Count);
        }
    }
}
