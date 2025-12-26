
using System;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmDeleteMajor : Form
    {
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        public frmDeleteMajor()
        {
            InitializeComponent();
        }

        private void frmDeleteMajor_Load(object sender, EventArgs e)
        {
            var faculties = facultyService.GetAll();
            cmbFaculty.DataSource = faculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";

            LoadMajors();
        }

        private void LoadMajors()
        {
            int facultyID;
            if (!int.TryParse(cmbFaculty.SelectedValue?.ToString(), out facultyID))
            {
                cmbMajor.DataSource = null;
                return;
            }

            var majors = majorService.GetMajorsByFaculty(facultyID);
            cmbMajor.DataSource = majors;
            cmbMajor.DisplayMember = "Name";
            cmbMajor.ValueMember = "MajorID";
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMajors();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có chuyên ngành để xóa không
                if (cmbMajor.Items.Count == 0)
                {
                    MessageBox.Show("Không có chuyên ngành nào để xóa!");
                    return;
                }

                int facultyID, majorID;
                if (!int.TryParse(cmbFaculty.SelectedValue?.ToString(), out facultyID) ||
                    !int.TryParse(cmbMajor.SelectedValue?.ToString(), out majorID))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Khoa và Chuyên ngành cần xóa!");
                    return;
                }

                // Kiểm tra ràng buộc sinh viên
                if (majorService.HasStudents(facultyID, majorID))
                {
                    MessageBox.Show("Không thể xóa chuyên ngành này vì đang có sinh viên theo học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa chuyên ngành này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    majorService.Delete(facultyID, majorID);
                    MessageBox.Show("Xóa thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMajors(); // Tải lại danh sách sau khi xóa
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
