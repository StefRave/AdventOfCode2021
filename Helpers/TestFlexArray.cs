using FluentAssertions;
using Xunit;

namespace AdventOfCode2019.Helpers
{
    public class TestFlexArray
    {
        [Fact]
        public void TestFlexArrayInserts()
        {
            FlexArray<char> sut;

            sut = new FlexArray<char> { DefaultValue = '.' };
            sut[0] = '1';
            sut.AsString().Should().Be("1");
            sut[-2] = '2';
            sut.AsString().Should().Be("2.1");
            sut[2] = '3';
            sut.AsString().Should().Be("2.1.3");
            sut[3] = '4';
            sut.AsString().Should().Be("2.1.34");
            sut[-3] = '5';
            sut.AsString().Should().Be("52.1.34");
            sut[-1] = '6';
            sut.AsString().Should().Be("5261.34");
            sut[1] = '7';
            sut.AsString().Should().Be("5261734");
        }

        [Fact]
        public void TestFlexArray2DInserts()
        {
            FlexArray2D<char> sut;

            sut = new FlexArray2D<char>() { Default2D = '.' };
            sut[0][0] = '1';
            sut.AsString().Should().Be("1");
            sut[-2][-2] = '2';
            sut.AsString().Should().Be("2..\n...\n..1");
        }
    }
}
