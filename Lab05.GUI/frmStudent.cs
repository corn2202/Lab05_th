
using GUI;
using Lab05.DAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lab05.GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        // Biến lưu trữ đường dẫn ảnh tạm thời
        private string _avatarPath = "";

        public frmStudent()
        {
            InitializeComponent();
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            // Chèn dòng "Chọn khoa" ở đầu
            listFacultys.Insert(0, new Faculty { FacultyID = 0, FacultyName = "-- Chọn Khoa --" });
            cmbFaculty.DataSource = listFacultys;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty?.FacultyName ?? "";
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore.ToString("F2");
                dgvStudent.Rows[index].Cells[4].Value = item.Major?.Name ?? "";
                ShowAvatar(item.Avatar);
            }
        }

        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                picAvatar.Image = null;
                return;
            }

            string solutionDir = Directory.GetParent(Application.StartupPath)?.Parent?.Parent?.FullName;
            if (string.IsNullOrEmpty(solutionDir))
            {
                picAvatar.Image = null;
                return;
            }

            string imagePath = Path.Combine(solutionDir, "Images", imageName);
            if (File.Exists(imagePath))
            {
                try
                {
                    picAvatar.Image = Image.FromFile(imagePath);
                }
                catch (Exception)
                {
                    picAvatar.Image = null;
                }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            List<Student> listStudents = chkUnregisterMajor.Checked
                ? studentService.GetAllHasNoMajor()
                : studentService.GetAll();

            BindGrid(listStudents);
        }

        // ==================== CHỨC NĂNG THÊM / CẬP NHẬT ====================
        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra thông tin bắt buộc
                if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                    string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtAverageScore.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra độ dài mã sinh viên
                if (txtStudentID.Text.Length != 10)
                {
                    MessageBox.Show("Mã sinh viên phải có 10 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra điểm trung bình
                if (!float.TryParse(txtAverageScore.Text, out float avgScore) || avgScore < 0 || avgScore > 10)
                {
                    MessageBox.Show("Điểm trung bình phải là số từ 0 đến 10!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy FacultyID từ ComboBox
                int facultyId = (int)cmbFaculty.SelectedValue;
                if (facultyId == 0) // Không chọn khoa
                {
                    MessageBox.Show("Vui lòng chọn khoa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng Student
                var student = new Student
                {
                    StudentID = txtStudentID.Text,
                    FullName = txtFullName.Text,
                    AverageScore = avgScore,
                    FacultyID = facultyId
                };

                // Xử lý avatar nếu có
                if (!string.IsNullOrEmpty(_avatarPath))
                {
                    string ext = Path.GetExtension(_avatarPath);
                    string fileName = $"{student.StudentID}{ext}";
                    string destFolder = Path.Combine(Application.StartupPath, "..\\..\\Images");
                    string destPath = Path.Combine(destFolder, fileName);

                    // Tạo thư mục nếu chưa tồn tại
                    Directory.CreateDirectory(destFolder);

                    // Sao chép file ảnh
                    File.Copy(_avatarPath, destPath, true);
                    student.Avatar = fileName;
                }

                // Gọi service để lưu (tự động phân biệt thêm/cập nhật)
                studentService.Save(student);

                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại dữ liệu
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== CHỨC NĂNG XÓA ====================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sinh viên có mã '{txtStudentID.Text}'?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    studentService.Delete(txtStudentID.Text);
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== CHỌN SINH VIÊN TRONG GRID ====================
        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvStudent.Rows.Count)
            {
                var row = dgvStudent.Rows[e.RowIndex];
                txtStudentID.Text = row.Cells["colStudentID"].Value?.ToString() ?? "";
                txtFullName.Text = row.Cells["colFullName"].Value?.ToString() ?? "";
                txtAverageScore.Text = row.Cells["colAvgScore"].Value?.ToString() ?? "";

                // Set Faculty
                string facultyName = row.Cells["colFaculty"].Value?.ToString() ?? "";
                var faculty = facultyService.GetAll().Find(f => f.FacultyName == facultyName);
                if (faculty != null)
                    cmbFaculty.SelectedValue = faculty.FacultyID;

                // Hiển thị avatar
                string avatar = row.Cells["colMajor"].Value?.ToString(); // Lưu ý: cột Major không chứa Avatar, nên dùng cách khác
                // Cách chính xác hơn: lấy từ đối tượng Student trong database
                using (var context = new StudentContext())
                {
                    var selectedStudent = context.Students.Find(txtStudentID.Text);
                    if (selectedStudent != null)
                        ShowAvatar(selectedStudent.Avatar);
                }
            }
        }

        // ==================== HÀM HỖ TRỢ ====================
        private void LoadData()
        {
            var listFacultys = facultyService.GetAll();
            var listStudents = studentService.GetAll();
            FillFacultyCombobox(listFacultys);
            BindGrid(listStudents);
        }

        private void ClearInputs()
        {
            txtStudentID.Clear();
            txtFullName.Clear();
            txtAverageScore.Clear();
            cmbFaculty.SelectedIndex = 0; // Reset về "-- Chọn Khoa --"
            picAvatar.Image = null;
            _avatarPath = "";
        }

        // ==================== CHỨC NĂNG UPLOAD ẢNH ====================
        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _avatarPath = ofd.FileName;
                    picAvatar.Image = Image.FromFile(_avatarPath);
                }
            }
        }

        private void xoáChuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new frmDeleteMajor();
            frm.ShowDialog();
        }

        private void đăngKíChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new frmRegister();
            frm.ShowDialog();
        }

        private void thêmChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new frmAddMajor();
            frm.ShowDialog();
        }

        private void sửaChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var editForm = new frmEditMajor())
            {
                editForm.Owner = this; // 👈 Gán owner để form con biết form cha
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Không cần làm gì thêm — vì form con đã gọi LoadMajors() tự động
                }
            }
        }
        
    }
}