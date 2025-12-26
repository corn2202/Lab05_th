using Lab05.DAL;
using System.Collections.Generic;
using System.Linq;

public class FacultyService
{
    public List<Faculty> GetAll()
    {
        using (var context = new StudentContext())
        {
            return context.Faculties.ToList();
        }
    }

    public Faculty GetById(int id)
    {
        using (var context = new StudentContext())
        {
            return context.Faculties.Find(id);
        }
    }


}