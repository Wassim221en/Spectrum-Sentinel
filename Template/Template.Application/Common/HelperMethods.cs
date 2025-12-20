namespace Template.Dashboard.Common;

public static class HelperMethods
{
    public static List<T> ApplyPagination<T>(this List<T> list,int pageSize,int pageIndex) where T:class
    =>list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
}