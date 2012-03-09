using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Diagnostics;

namespace MLifterUnbind
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Learning Module (*.mlm)|*.mlm";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SqlCeConnection con = null;
                try
                {
                    con = new SqlCeConnection(string.Format("Data Source={0};Max Database Size=4000;Persist Security Info=False;", ofd.FileName));
                    con.Open();
                    SqlCeCommand cmd = con.CreateCommand();
                    cmd.CommandText = "UPDATE UserProfiles SET local_directory_id=NULL;";
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("All user profile bindings where removed from the selected learning module.", "Bindings removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlCeException ex)
                {
                    switch (ex.NativeError) //Error-List: http://msdn.microsoft.com/en-us/library/ms171879(SQL.90).aspx
                    {
                        case 25079:             //25079 --> SSCE_M_ENCRYPTEDDBMUSTHAVEPWD --> A password must be specified when a database is created by using encryption.
                        case 25028:             //25028	--> SSCE_M_INVALIDPASSWORD  --> The specified password does not match the database password.
                            MessageBox.Show("This tool can not unbind protecte learning modules. Please contact support@memorylifter.com for help.", "Cannot unbind protected modules",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 25035:             //25035 --> SSCE_M_FILESHAREVIOLATION   --> There is a file-sharing violation. A different process might be using the file.
                            MessageBox.Show("Error opening the learning module - another program is accessing the file. Please close any other programs and try again", "Error accessing file",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
                        default:
                            MessageBox.Show("Error unbinding learning module: " + ex.NativeError + Environment.NewLine + "Please contact support@memorylifter.com", "Error " + ex.NativeError,
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Error unbinding learning module: " + exp.Message + Environment.NewLine + "Please contact support@memorylifter.com", "Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trace.WriteLine(exp.ToString());
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }
    }
}
