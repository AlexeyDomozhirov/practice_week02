namespace task11tests;

using CalcConstructor;
using ICalculator;
using Xunit;

public class DynamicCalculatorTests
{
    private readonly ICalc _calculator;

    public DynamicCalculatorTests()
    {
        _calculator = DynamicCalculator.Create();
    }

    [Fact]
    public void Add_ReturnsCorrectSum()
    {
        Assert.Equal(15, _calculator.Add(10, 5));
        Assert.Equal(0, _calculator.Add(-5, 5));
        Assert.Equal(-10, _calculator.Add(-7, -3));
    }

    [Fact]
    public void Minus_ReturnsCorrectDifference()
    {
        Assert.Equal(5, _calculator.Minus(10, 5));
        Assert.Equal(15, _calculator.Minus(10, -5));
        Assert.Equal(-10, _calculator.Minus(-5, 5));
    }

    [Fact]
    public void Mul_ReturnsCorrectProduct()
    {
        Assert.Equal(50, _calculator.Mul(10, 5));
        Assert.Equal(0, _calculator.Mul(10, 0));
        Assert.Equal(-12, _calculator.Mul(3, -4));
    }

    [Fact]
    public void Div_ReturnsCorrectQuotient()
    {
        Assert.Equal(4, _calculator.Div(20, 5));
        Assert.Equal(5, _calculator.Div(25, 5));
        Assert.Equal(-6, _calculator.Div(-30, 5));
    }

    [Fact]
    public void Div_ByZero_ThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Div(10, 0));
    }

    [Theory]
    [InlineData(10, 5, 15)]
    [InlineData(0, 0, 0)]
    [InlineData(-8, 3, -5)]
    public void Add_WorksWithVariousInputs(int a, int b, int expected)
    {
        Assert.Equal(expected, _calculator.Add(a, b));
    }
}
