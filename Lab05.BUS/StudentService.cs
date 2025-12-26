using Lab05.DAL;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class StudentService
{
    public List<Student> GetAll()
    {
        using (var context = new StudentContext())
        {
            return context.Students
                .Include(s => s.Major)
                .Include(s => s.Faculty)
                .ToList();
        }
    }

    public List<Student> GetAllHasNoMajor()
    {
        using (var context = new StudentContext())
        {
            return context.Students
                .Where(s => s.MajorID == null)
                .Include(s => s.Faculty)
                .ToList();
        }
    }

    public Student GetById(string id)
    {
        using (var context = new StudentContext())
        {
            return context.Students.Find(id);
        }
    }

    public void Delete(string studentID)
    {
        using (var context = new StudentContext())
        {
            var student = context.Students.Find(studentID);
            if (student != null)
            {
                context.Students.Remove(student);
                context.SaveChanges();
            }
        }
    }

    public void Save(Student student)
    {
        using (var context = new StudentContext())
        {
            if (context.Students.Any(s => s.StudentID == student.StudentID))
            {
                context.Entry(student).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Students.Add(student);
            }
            context.SaveChanges();
        }
    }
}