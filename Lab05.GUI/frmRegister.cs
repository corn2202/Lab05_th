
using Lab05.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lab05.GUI
{
    public partial class frmRegister : Form
    {
        private readonly FacultyService _facultyService = new FacultyService();
        private readonly MajorService _majorService = new MajorService();
        private readonly StudentService _studentService = new StudentService();

        public frmRegister()
        {
            InitializeComponent();
        }

        private void frmRegister_Load(object sender, EventArgs e)
        {
            LoadFaculties();
        }

        private void LoadFaculties()
        {
            var faculties = _facultyService.GetAll();
            cmbFaculty.DataSource = faculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
            cmbFaculty.SelectedIndex = -1; // Không chọn gì ban đầu
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFaculty.SelectedValue == null || cmbFaculty.SelectedValue.ToString() == "0")
                return;

            // Lấy đối tượng Faculty được chọn
            var selectedFaculty = cmbFaculty.SelectedItem as Faculty;

            // Kiểm tra null để tránh lỗi
            if (selectedFaculty == null)
                return;

            int facultyId = selectedFaculty.FacultyID; // Lấy FacultyID từ đối tượng
            var majors = _majorService.GetByFaculty(facultyId);
            cmbMajor.DataSource = majors;
            cmbMajor.DisplayMember = "MajorName";
            cmbMajor.ValueMember = "MajorID";
            cmbMajor.SelectedIndex = -1;

            // Tải danh sách sinh viên chưa có chuyên ngành của khoa này
            LoadStudentsWithoutMajor(facultyId);
        }

        private void LoadStudentsWithoutMajor(int facultyId)
        {
            var students = _studentService.GetAllHasNoMajor();
            var filteredStudents = students.Where(s => s.FacultyID == facultyId).ToList();

            dgvStudent.Rows.Clear();
            foreach (var s in filteredStudents)
            {
                int idx = dgvStudent.Rows.Add();
                dgvStudent.Rows[idx].Cells["colSelect"].Value = false; // Mặc định không chọn
                dgvStudent.Rows[idx].Cells["colStudentID"].Value = s.StudentID;
                dgvStudent.Rows[idx].Cells["colFullName"].Value = s.FullName;
                dgvStudent.Rows[idx].Cells["colFaculty"].Value = s.Faculty?.FacultyName ?? "";
                dgvStudent.Rows[idx].Cells["colAverageScore"].Value = s.AverageScore.ToString("F2");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cmbMajor.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn chuyên ngành!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int majorId = (int)cmbMajor.SelectedValue;

            List<string> selectedStudentIDs = new List<string>();
            foreach (DataGridViewRow row in dgvStudent.Rows)
            {
                if (row.Cells["colSelect"].Value != null && (bool)row.Cells["colSelect"].Value)
                {
                    selectedStudentIDs.Add(row.Cells["colStudentID"].Value?.ToString());
                }
            }

            if (selectedStudentIDs.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (string studentID in selectedStudentIDs)
                {
                    var student = _studentService.GetAll().FirstOrDefault(s => s.StudentID == studentID);
                    if (student != null)
                    {
                        student.MajorID = majorId;
                        _studentService.Save(student); // Cập nhật lại sinh viên
                    }
                }

                MessageBox.Show("Đăng ký chuyên ngành thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudentsWithoutMajor((int)cmbFaculty.SelectedValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}