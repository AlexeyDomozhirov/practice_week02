namespace task07;

[VersionAttribute(1, 0), DisplayNameAttribute("Пример класса")]
public class SampleClass
{
    [DisplayNameAttribute("Тестовый метод")]
    public void TestMethod() { }

    [DisplayNameAttribute("Числовое свойство")]
    public int Number { get; }
}
