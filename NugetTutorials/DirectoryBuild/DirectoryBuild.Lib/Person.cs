using FluentValidation;

namespace DirectoryBuild.Lib;

public class Person
{
    public Person(string name, DateOnly? birthDay)
    {
        Id = Guid.NewGuid();
        Name = name;
        BirthDay = birthDay;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateOnly? BirthDay { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, BirthDay: {BirthDay}";
    }
}


public class PersonValidation : AbstractValidator<Person>
{
    public PersonValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
}