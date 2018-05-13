using System;
using System.Reflection;

namespace Zutrittkontrolle_Zeppelin_Rental.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}