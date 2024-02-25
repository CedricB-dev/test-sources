// See https://aka.ms/new-console-template for more information

using DirectoryBuild.Lib;

var person = new Person("John Doe", new DateOnly(1980, 1, 1));
var validator = new PersonValidation();
var result = validator.Validate(person);


Console.WriteLine("Hello, {0}", person);