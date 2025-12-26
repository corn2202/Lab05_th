using Lab05.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

public class MajorService
{
    public List<Major> GetAll()
    {
        using (var context = new StudentContext())
        {
            return context.Majors.ToList();
        }
    }

    public List<Major> GetByFaculty(int facultyId)
    {
        using (var context = new StudentContext())
        {
            return context.Majors
                          .Where(m => m.FacultyID == facultyId)
                          .ToList();
        }
    }

    // 👇 THÊM PHƯƠNG THỨC NÀY VÀO ĐÂY
    public void Add(string majorName, int facultyId)
    {
        using (var context = new StudentContext())
        {
            // Kiểm tra trùng tên chuyên ngành trong khoa
            if (context.Majors.Any(m => m.FacultyID == facultyId && m.Name == majorName))
            {
                throw new InvalidOperationException("Chuyên ngành này đã tồn tại trong khoa này!");
            }

            // Tự sinh MajorID mới cho khoa đó
            int newMajorId = 1;
            var existingMajors = context.Majors.Where(m => m.FacultyID == facultyId).ToList();
            if (existingMajors.Count > 0)
            {
                newMajorId = existingMajors.Max(m => m.MajorID) + 1;
            }

            var newMajor = new Major
            {
                FacultyID = facultyId,
                MajorID = newMajorId,
                Name = majorName
            };

            context.Majors.Add(newMajor);
            context.SaveChanges();
        }
    }


    public void Update(int facultyId, int majorId, string newName)
    {
        using (var context = new StudentContext())
        {
            var major = context.Majors
                               .FirstOrDefault(m => m.FacultyID == facultyId && m.MajorID == majorId);
            if (major == null)
                throw new InvalidOperationException("Không tìm thấy chuyên ngành để cập nhật.");

            // Kiểm tra trùng tên trong cùng khoa (nếu cần)
            if (context.Majors.Any(m => m.FacultyID == facultyId && m.Name == newName &&
                                       !(m.FacultyID == facultyId && m.MajorID == majorId)))
            {
                throw new InvalidOperationException("Tên chuyên ngành đã tồn tại trong khoa này!");
            }

            major.Name = newName;
            context.SaveChanges();
        }
    }

    public List<Major> GetMajorsByFaculty(int facultyId)
    {
        using (var context = new StudentContext())
        {
            return context.Majors
                          .Where(m => m.FacultyID == facultyId)
                          .ToList();
        }
    }

    public Major FindById(int facultyId, int majorId)
    {
        using (var context = new StudentContext())
        {
            return context.Majors
                          .FirstOrDefault(m => m.FacultyID == facultyId && m.MajorID == majorId);
        }
    }



    public void Update(int oldFacultyId, int oldMajorId, string newName, int newFacultyId)
    {
        using (var context = new StudentContext())
        {
            var major = context.Majors
                               .FirstOrDefault(m => m.FacultyID == oldFacultyId && m.MajorID == oldMajorId);
            if (major == null)
                throw new InvalidOperationException("Không tìm thấy chuyên ngành để cập nhật.");

            // Kiểm tra trùng tên trong khoa mới
            if (context.Majors.Any(m => m.FacultyID == newFacultyId && m.Name == newName &&
                                       !(m.FacultyID == oldFacultyId && m.MajorID == oldMajorId)))
            {
                throw new InvalidOperationException("Tên chuyên ngành đã tồn tại trong khoa này!");
            }

            major.Name = newName;
            major.FacultyID = newFacultyId; // Cho phép đổi khoa

            context.SaveChanges();
        }
    }

    public bool HasStudents(int facultyId, int majorId)
    {
        using (var context = new StudentContext())
        {
            return context.Students.Any(s => s.FacultyID == facultyId && s.MajorID == majorId);
        }
    }


    public void Delete(int facultyId, int majorId)
    {
        using (var context = new StudentContext())
        {
            var major = context.Majors
                               .FirstOrDefault(m => m.FacultyID == facultyId && m.MajorID == majorId);
            if (major == null)
                throw new InvalidOperationException("Không tìm thấy chuyên ngành để xóa.");

            // Kiểm tra ràng buộc — nếu có sinh viên, nên xóa trước hoặc ném exception
            if (context.Students.Any(s => s.FacultyID == facultyId && s.MajorID == majorId))
            {
                throw new InvalidOperationException("Không thể xóa vì đang có sinh viên theo học.");
            }

            context.Majors.Remove(major);
            context.SaveChanges();
        }
    }
}