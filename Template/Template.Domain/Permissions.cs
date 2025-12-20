using System.Reflection;

namespace Template.Domain
{
    public static class Permissions
    {
        public static class Employees
        {
            public const string View = "Employees.View";
            public const string Create = "Employees.Create";
            public const string Update = "Employees.Update";
            public const string Delete = "Employees.Delete";
        }
        public static class Roles
        {
            public const string View = "Roles.View";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
        }
        public static List<PermissionsPage> GetAllPermissions()
        {
            var permissionsPages = new List<PermissionsPage>();
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var type in nestedTypes)
            {
                var permissionsPage = new PermissionsPage();
                permissionsPage.Page = type.Name;
                permissionsPage.Permissions = new();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(string))
                    {
                        var value = field.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(value))
                            permissionsPage.Permissions.Add(value);
                    }
                }
                permissionsPages.Add(permissionsPage);
            }

            return permissionsPages;
        }
    }

    public class PermissionsPage
    {
        public string Page { get; set; }
        public List<string>Permissions { get; set; }
    }
}