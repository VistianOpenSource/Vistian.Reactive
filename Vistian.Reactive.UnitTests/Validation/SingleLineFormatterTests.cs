using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Validation;
using Xunit;

namespace Vistian.Reactive.UnitTests.Validation
{
    public class SingleLineFormatterTests
    {
        private const string Item1Text = "item1";
        private const string Item2Text = "item2";

        [Fact]
        public void DefaultCorrectFormatTest()
        {
            var f = SingleLineFormatter.Default;

            var vt = new ValidationText(new [] { new ValidationText(Item1Text),new ValidationText(Item2Text) });

            var sl = f.Format(vt);

            Assert.Equal(Item1Text+","+Item2Text,sl);
        }

        [Fact]
        public void CustomSeparatorTest()
        {
            var f = new SingleLineFormatter("+");

            var vt = new ValidationText(new[] { new ValidationText(Item1Text), new ValidationText(Item2Text) });

            var sl = f.Format(vt);

            Assert.Equal(Item1Text + "+" + Item2Text, sl);

        }
    }
}
