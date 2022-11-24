using System;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Vorbis;
using NVorbis;
using System.IO.Compression;
using System.Diagnostics;
using System.Security.Policy;

namespace facade_editor
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext synchronizationContext; //this is needed for updating the UI while doing tasks on another thread
        string version = "1.0.0";
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            initializeAndReadSettings();
            if (IsCurrentUserInAdminGroup()) Text = "Façade editor (Admin rights) " + version;
            else Text = "Façade editor " + version;

        }
        string[] names = new string[100000]; //used for storing path for the files to randomize, yes I will change it to list or something else later
        string path = @""; //path to game
        int i = 0;

        async void initializeAndReadSettings()
        {
            if (File.Exists("settings.cfg"))
            {
                path = File.ReadAllText("settings.cfg");
                pathTextbox.Text = path;
                try
                {
                    checkAdvancedSettings();
                    await GenerateBackup();
                }
                catch
                {
                    MessageBox.Show("The path might be wrong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        private bool IsCurrentUserInAdminGroup() //check if run as admin
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }


        public async void randomizeButton_Click(object sender, EventArgs e)
        {
            disableButtons();
            if (soundsCheckBox.Checked || texturesTextBox.Checked || cursorsCheckBox.Checked || animationsCheckBox.Checked)
            {
                if (soundsCheckBox.Checked) await randomizeSounds();
                if (texturesTextBox.Checked) await randomizeTextures();
                if (cursorsCheckBox.Checked) await randomizeCursors();
                if (animationsCheckBox.Checked) await randomizeAnimations();
            }
            else MessageBox.Show("You need to select what you want to randomize with the checkboxes first.", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Information);
            enableButtons();
        }
        private async void restoreButton_Click(object sender, EventArgs e)
        {
            disableButtons();
            if (soundsCheckBox.Checked || texturesTextBox.Checked || cursorsCheckBox.Checked || animationsCheckBox.Checked)
                await Restore();
            else
                MessageBox.Show("You need to select what you want to restore with the checkboxes first.", "Wait");
            enableButtons();
        }
        async Task Restore()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (soundsCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring sounds..");
                        CopyFilesRecursively(path + @"Backup\Sounds", path + @"Sounds", true);
                    }
                    if (texturesTextBox.Checked)
                    {
                        UpdateUI("", "Restoring textures..");
                        try
                        {
                            var di = new DirectoryInfo(path + @"textures");
                            foreach (var file in di.GetFiles("*", SearchOption.AllDirectories)) //I need to remove the read-only attribute of the pictures, otherwise the program can't access them
                                file.Attributes &= ~FileAttributes.ReadOnly;
                            var di2 = new DirectoryInfo(path + @"Backup\textures");
                            foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                                file.Attributes &= ~FileAttributes.ReadOnly;
                        }
                        catch { }

                        CopyFilesRecursively(path + @"Backup\textures", path + @"textures", true);
                    }
                    if (cursorsCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring cursors..");
                        CopyFilesRecursively(path + @"Backup\cursors", path + @"cursors", true);
                    }
                    if (animationsCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring animations..");
                        CopyFilesRecursively(path + @"Backup\animation", path + @"animation", true);
                    }
                    UpdateUI(" ", "Files restored.");
                }
                catch (Exception ex)
                {
                    UpdateUI("", "Error restoring: " + ex.Message);
                }
            });


        }
        void disableButtons()
        {
            randomizeButton.Enabled = false;
            replaceButton.Enabled = false;
            browseButton.Enabled = false;
            replaceBrowseButton.Enabled = false;
            clearListButton.Enabled = false;
            restoreButton.Enabled = false;
            launchButton.Enabled = false;
        }
        void enableButtons()
        {
            randomizeButton.Enabled = true;
            replaceButton.Enabled = true;
            browseButton.Enabled = true;
            replaceBrowseButton.Enabled = true;
            clearListButton.Enabled = true;
            if (useBackupFilesRadioButton.Enabled)
                restoreButton.Enabled = true;
            if (javaDebugCheckBox.Checked)
                launchButton.Enabled = true;
        }
        async Task GenerateBackup()
        {
            UpdateUI("", "Checking for backup..");
            try
            {
                if (!Directory.Exists(path + @"Backup"))
                {
                    UpdateUI("", "Backup not found");
                    if (MessageBox.Show(@"Do you want to generate a backup? You can use those original files to restore later. The backup will be saved at Facade\util\sources\facade\Backup.", "Backup is very recommended", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        disableButtons();
                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(path + @"Backup\animation")) Directory.CreateDirectory(path + @"Backup\animation");
                            CopyFilesRecursively(path + @"animation", path + @"Backup\animation", false);
                            if (!Directory.Exists(path + @"Backup\cursors")) Directory.CreateDirectory(path + @"Backup\cursors");
                            CopyFilesRecursively(path + @"cursors", path + @"Backup\cursors", false);
                            if (!Directory.Exists(path + @"Backup\Sounds")) Directory.CreateDirectory(path + @"Backup\Sounds");
                            CopyFilesRecursively(path + @"Sounds", path + @"Backup\Sounds", false);
                            if (!Directory.Exists(path + @"Backup\textures")) Directory.CreateDirectory(path + @"Backup\textures");
                            CopyFilesRecursively(path + @"textures", path + @"Backup\textures", false);
                        });
                        UpdateUI(" ", "Backup generated.");
                        useBackupFilesRadioButton.Enabled = true;
                        enableButtons();
                    }

                }
                else
                {
                    UpdateUI("", "Backup found.");
                    useBackupFilesRadioButton.Enabled = true;
                    enableButtons();
                }

            }
            catch (Exception ex)
            {
                UpdateUI(" ", "Error generating backup: " + ex.Message);
                UpdateUI(" ", @"Please remove the backup folder from Facade\util\sources\facade\ and try again.");
            }


        }

        async Task randomizeCursors()
        {
            i = 0;
            UpdateUI("", "Randomizing cursors..");

            await Task.Run(() =>
            {
                try
                {
                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        foreach (string file in Directory.EnumerateFiles(path + @"cursors", "*.ico", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                        if (!Directory.Exists(path + @"cursors_r")) Directory.CreateDirectory(path + @"cursors_r");
                    }
                    else
                        foreach (string file in Directory.EnumerateFiles(path + @"Backup\cursors", "*.ico", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }


                    Random random = new Random();
                    int rndnumber;
                    int[] rnd = new int[i];
                    string[] rndnames = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        rndnumber = random.Next(1, i + 1);
                        if (!rnd.Contains(rndnumber))
                        {
                            rnd[j] = rndnumber;
                            if (dontUseFilesFromBackupRadioButton.Checked)
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"cursors", "cursors_r");
                                File.Move(names[j], rndnames[j]);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"cursors\") + 8) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"cursors_r\") + 10) + " " + (j + 1) + "/" + i, "");
                            }
                            else
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"Backup\cursors", "cursors");
                                File.Copy(names[j], rndnames[j], true);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"cursors\") + 8) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"cursors\") + 8) + " " + (j + 1) + "/" + i, "");
                            }


                        }
                        else j--;
                    }
                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        Directory.Delete(path + @"cursors", true);
                        Directory.Move(path + @"cursors_r", path + @"cursors");
                    }
                    UpdateUI(" ", "Cursors randomized succesfully.");
                }
                catch (Exception ex)
                {
                    UpdateUI(" ", "Error randomizing cursors: " + ex.Message);
                }
            });


        }
        private void CopyFilesRecursively(string sourcePath, string targetPath, bool restoreOrBackup) //bool is only used to say restoring or backing up
        {
            //create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            //copy all the files
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                if (restoreOrBackup)
                    UpdateUI("Restoring " + newPath + "..", "");
                else
                    UpdateUI("Backing up " + newPath + "..", "");
            }
        }
        async Task randomizeSounds()
        {
            i = 0;

            await Task.Run(() =>
            {
                try
                {
                    UpdateUI("", "Randomizing Sounds..");
                    if (dontUseFilesFromBackupRadioButton.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"Sounds", "*.wav", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                    else
                        foreach (string file in Directory.EnumerateFiles(path + @"Backup\Sounds", "*.wav", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }

                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        if (Directory.Exists(path + @"Sounds_r")) Directory.Delete(path + @"Sounds_r", true);
                        Directory.CreateDirectory(path + @"Sounds_r");
                        Directory.CreateDirectory(path + @"Sounds_r\global");
                        Directory.CreateDirectory(path + @"Sounds_r\grace");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\01");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\02");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\03");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\04");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\05");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\06");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\07");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\08");
                        Directory.CreateDirectory(path + @"Sounds_r\grace\09");
                        Directory.CreateDirectory(path + @"Sounds_r\trip");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\01");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\02");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\03");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\04");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\05");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\06");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\07");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\08");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\09");
                        Directory.CreateDirectory(path + @"Sounds_r\trip\10");
                        Directory.CreateDirectory(path + @"Sounds_r\mp3_1");
                        Directory.CreateDirectory(path + @"Sounds_r\mp3_2");
                        File.Copy(path + @"Sounds\global\globalsnd.txt", path + @"Sounds_r\global\globalsnd.txt");
                        File.Copy(path + @"Sounds\grace\gracesnd.txt", path + @"Sounds_r\grace\gracesnd.txt");
                        File.Copy(path + @"Sounds\trip\tripsnd.txt", path + @"Sounds_r\trip\tripsnd.txt");
                        CopyFilesRecursively(path + @"Sounds\mp3_1", path + @"Sounds_r\mp3_1", false);
                        CopyFilesRecursively(path + @"Sounds\mp3_2", path + @"Sounds_r\mp3_2", false);
                    }

                    Random random = new Random();
                    int rndnumber;
                    int[] rnd = new int[i];
                    string[] rndnames = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        rndnumber = random.Next(1, i + 1);
                        if (!rnd.Contains(rndnumber))
                        {
                            rnd[j] = rndnumber;
                            if (dontUseFilesFromBackupRadioButton.Checked)
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"Sounds", "Sounds_r");
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"Sounds\") + 7) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"Sounds_r\") + 9) + " " + (j + 1) + "/" + i, "");
                                File.Move(names[j], rndnames[j]);
                            }
                            else
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"Backup\Sounds", "Sounds");
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"Sounds\") + 7) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"Sounds\") + 7) + " " + (j + 1) + "/" + i, "");
                                File.Copy(names[j], rndnames[j], true);
                            }

                        }
                        else j--;
                    }
                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        Directory.Delete(path + @"Sounds", true);
                        Directory.Move(path + @"Sounds_r", path + @"Sounds");
                    }
                    UpdateUI(" ", "Succesfully randomized the sound files.");
                }
                catch (Exception ex)
                {
                    UpdateUI(" ", "Error randomizing sounds: " + ex.Message);
                }
            });


        }

        public void UpdateUI(string progress, string textboxmessage) // updates the textbox and label at the bottom
        {
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                if (progress != "") label1.Text = progress;
                if (textboxmessage != "") randomizeLogTextBox.AppendText(textboxmessage + Environment.NewLine);
            }), "");
        }
        async Task randomizeTextures()
        {
            i = 0;

            await Task.Run(() =>
            {
                try
                {
                    if (lettersTextureIntactCheckBox.Checked)
                    {
                        File.Move(path + @"textures\yellowfont.bmp", path + @"yellowfont.bmp");
                        File.Move(path + @"textures\yellowfont2.bmp", path + @"yellowfont2.bmp");
                    }
                    //UpdateUI("", "Removing readonly attributes from texture files..");
                    var di = new DirectoryInfo(path + @"textures");
                    foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))         //I have to remove the read-only attribute of the pictures, otherwise the program can't access them
                        file.Attributes &= ~FileAttributes.ReadOnly;
                    UpdateUI("", "Randomizing textures..");
                    if (dontUseFilesFromBackupRadioButton.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"textures", "*.bmp", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                    else
                        foreach (string file in Directory.EnumerateFiles(path + @"Backup\textures", "*.bmp", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }

                    if (dontUseFilesFromBackupRadioButton.Checked)
                        if (!Directory.Exists(path + @"textures_r")) Directory.CreateDirectory(path + @"textures_r");

                    Random random = new Random();
                    int rndnumber;
                    int[] rnd = new int[i];
                    string[] rndnames = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        rndnumber = random.Next(1, i + 1);
                        if (!rnd.Contains(rndnumber))
                        {
                            rnd[j] = rndnumber;
                            if (dontUseFilesFromBackupRadioButton.Checked)
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"textures", "textures_r");
                                File.Move(names[j], rndnames[j]);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"textures\") + 9) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"textures_r\") + 11) + " " + (j + 1) + "/" + i, "");
                            }
                            else
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"Backup\textures", "textures");
                                File.Copy(names[j], rndnames[j], true);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"textures\") + 9) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"textures\") + 9) + " " + (j + 1) + "/" + i, "");
                            }

                        }
                        else j--;
                    }
                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        Directory.Delete(path + @"textures", true);
                        Directory.Move(path + @"textures_r", path + @"textures");
                    }
                    if (lettersTextureIntactCheckBox.Checked)
                    {
                        File.Move(path + @"yellowfont.bmp", path + @"textures\yellowfont.bmp");
                        File.Move(path + @"yellowfont2.bmp", path + @"textures\yellowfont2.bmp");
                    }
                    UpdateUI(" ", "Textures randomized succesfully.");
                }
                catch (Exception ex)
                {
                    UpdateUI(" ", "Error randomizing textures: " + ex.Message);
                }
            });


        }
        async Task randomizeAnimations()
        {
            i = 0;
            UpdateUI("", "Randomizing animations..");

            await Task.Run(() =>
            {
                try
                {
                    if (dontUseFilesFromBackupRadioButton.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"animation", "*.abinary", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                    else
                        foreach (string file in Directory.EnumerateFiles(path + @"Backup\animation", "*.abinary", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }


                    if (!Directory.Exists(path + @"animation_r")) Directory.CreateDirectory(path + @"animation_r");
                    if (!Directory.Exists(path + @"animation_r\grace")) Directory.CreateDirectory(path + @"animation_r\grace");
                    if (!Directory.Exists(path + @"animation_r\trip")) Directory.CreateDirectory(path + @"animation_r\trip");
                    if (!File.Exists(path + @"animation_r\grace\grace.aindex")) File.Copy(path + @"animation\grace\grace.aindex", path + @"animation_r\grace\grace.aindex");
                    if (!File.Exists(path + @"animation_r\grace\graceScript.sbinary")) File.Copy(path + @"animation\grace\graceScript.sbinary", path + @"animation_r\grace\graceScript.sbinary");
                    if (!File.Exists(path + @"animation_r\trip\trip.aindex")) File.Copy(path + @"animation\trip\trip.aindex", path + @"animation_r\trip\trip.aindex");
                    if (!File.Exists(path + @"animation_r\trip\tripScript.sbinary")) File.Copy(path + @"animation\trip\tripScript.sbinary", path + @"animation_r\trip\tripScript.sbinary");

                    Random random = new Random();
                    int rndnumber;
                    int[] rnd = new int[i];
                    string[] rndnames = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        rndnumber = random.Next(1, i + 1);
                        if (!rnd.Contains(rndnumber))
                        {
                            rnd[j] = rndnumber;
                            rndnames[j] = names[rndnumber - 1].Replace("animation", "animation_r");
                            if (dontUseFilesFromBackupRadioButton.Checked)
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"animation", "animation_r");
                                File.Move(names[j], rndnames[j]);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"animation\") + 10) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"animation_r\") + 12) + " " + (j + 1) + "/" + i, "");
                            }
                            else
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"Backup\animation", "animation");
                                File.Copy(names[j], rndnames[j], true);
                                UpdateUI(names[j].Remove(0, names[j].IndexOf(@"animation\") + 10) + " to " + rndnames[j].Remove(0, rndnames[j].IndexOf(@"animation\") + 10) + " " + (j + 1) + "/" + i, "");
                            }
                        }
                        else j--;
                    }
                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        Directory.Delete(path + @"animation", true);
                        Directory.Move(path + @"animation_r", path + @"animation");
                    }
                    UpdateUI(" ", "Animations randomized succesfully.");
                }
                catch (Exception ex)
                {
                    UpdateUI(" ", "Error randomizing animations: " + ex.Message);
                }
            });


            /* i = 0;
             UpdateUI("", "Replacing animations..");
             await Task.Run(() =>
             {
                 foreach (string file in Directory.EnumerateFiles(path + @"animation\grace", "*.abinary", SearchOption.AllDirectories))
                 {
                     names[i] = file;
                     UpdateUI(names[i], "");
                     i++;
                 }


                 if (!Directory.Exists(path + @"animation_r")) Directory.CreateDirectory(path + @"animation_r");
                 if (!Directory.Exists(path + @"animation_r\grace")) Directory.CreateDirectory(path + @"animation_r\grace");
                 if (!File.Exists(path + @"animation_r\grace\grace.aindex")) File.Copy(path + @"animation\grace\grace.aindex", path + @"animation_r\grace\grace.aindex");
                 if (!File.Exists(path + @"animation_r\grace\graceScript.sbinary")) File.Copy(path + @"animation\grace\graceScript.sbinary", path + @"animation_r\grace\graceScript.sbinary");


                 Random random = new Random();
                 int rndnumber;
                 int[] rnd = new int[i];
                 string[] rndnames = new string[i];
                 for (int j = 0; j < i; j++)
                 {
                     rndnumber = random.Next(1, i + 1);
                     if (!rnd.Contains(rndnumber))
                     {
                         rnd[j] = rndnumber;
                         rndnames[j] = names[rndnumber - 1].Replace("animation", "animation_r");
                         File.Copy(names[j], rndnames[j]);
                         UpdateUI(names[j] + " random " + rndnames[j] + " " + j + "/" + i, "");
                     }
                     else j--;
                 }
             });
             UpdateUI("", "Animations replaced succesfully.");*/
        }


        private async void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                useBackupFilesRadioButton.Enabled = false;
                disableButtons();
                if (folderBrowserDialog1.SelectedPath.Contains("Facade") || folderBrowserDialog1.SelectedPath.Contains("Façade"))
                {
                    if (folderBrowserDialog1.SelectedPath.Contains(@"\util\sources\facade"))
                        path = folderBrowserDialog1.SelectedPath + @"\";
                    else if (folderBrowserDialog1.SelectedPath.Contains(@"\util\sources"))
                        path = folderBrowserDialog1.SelectedPath + @"\facade\";
                    else if (folderBrowserDialog1.SelectedPath.Contains(@"\util"))
                        path = folderBrowserDialog1.SelectedPath + @"\sources\facade\";
                    else
                        path = folderBrowserDialog1.SelectedPath + @"\util\sources\facade\";

                    pathTextbox.Text = path;
                    if (path.Contains("Program Files") && !IsCurrentUserInAdminGroup())
                    {
                        MessageBox.Show("Program Files folder detected, I might not have permission to write there without administrator rights.", "A little warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        warningLabel.Text = "I might not have permission to write to that folder. Please try to run this program with administrator rights\nor move the game if the program fails to run properly.";
                    }
                    else warningLabel.Text = "";
                    await GenerateBackup();
                }

                else { warningLabel.Text = "Façade folder not detected. Are you sure you've selected the right folder?"; path = folderBrowserDialog1.SelectedPath + @"\"; pathTextbox.Text = path; }
                enableButtons();
                try
                {
                    File.WriteAllText("settings.cfg", path); // will write better saving method if that's ever needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error trying to save settings: " + ex.Message);
                }
                try
                {
                    checkAdvancedSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Advanced settings failed to initialize: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void clearListButton_Click(object sender, EventArgs e)
        {
            customSoundCount = 0;
            clearListButton.Enabled = false;
            soundListLabel.Text = "List cleared.";
            UpdateUIReplaceTab("", "List cleared.");
        }


        string[] customSoundNames = new string[100000];
        int customSoundCount = 0;
        int mp3 = 0, wav = 0, ogg = 0; // these are just to show how much of these file types were found
        private void replaceBrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                soundListLabel.Text = "Searching for sound files..";
                clearListButton.Enabled = true;

                try
                {
                    foreach (string file in Directory.EnumerateFiles(folderBrowserDialog2.SelectedPath, "*.mp3", SearchOption.AllDirectories))
                    {
                        customSoundNames[customSoundCount] = file;
                        customSoundCount++;
                        mp3++;
                    }
                    foreach (string file in Directory.EnumerateFiles(folderBrowserDialog2.SelectedPath, "*.wav", SearchOption.AllDirectories))
                    {
                        customSoundNames[customSoundCount] = file;
                        customSoundCount++;
                        wav++;
                    }
                    foreach (string file in Directory.EnumerateFiles(folderBrowserDialog2.SelectedPath, "*.ogg", SearchOption.AllDirectories))
                    {
                        customSoundNames[customSoundCount] = file;
                        customSoundCount++;
                        ogg++;
                    }
                    UpdateUIReplaceTab("", wav + " wav files, " + mp3 + " mp3 files and " + ogg + " ogg files were found in " + folderBrowserDialog2.SelectedPath);
                    soundListLabel.Text = customSoundCount + " sound files were found. Click browse if you want to add more from other sources.";
                }
                catch (Exception ex)
                {
                    soundListLabel.Text = customSoundCount + " sound files were found, but an error occured while searching for the files.\nClick browse if you want to add more from other sources.";
                    UpdateUIReplaceTab("", wav + " wav files, " + mp3 + " mp3 files and " + ogg + " ogg files were found in " + folderBrowserDialog2.SelectedPath + ", but with an error: " + ex.Message);
                }
            }
            mp3 = 0; wav = 0; ogg = 0;

        }

        private async void replaceButton_Click(object sender, EventArgs e)
        {
            disableButtons();
            if (customSoundCount != 0)
                await replaceSounds();
            else
                UpdateUIReplaceTab("", "No custom sound files");
            enableButtons();
        }
        async Task replaceSounds()
        {
            i = 0; //!!!
            string[] tempcustomSoundNames = new string[10000];
            int randomlyCopied = 0;

            await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(path + @"temp_sounds")) Directory.CreateDirectory(path + @"temp_sounds");
                    UpdateUIReplaceTab("", "Converting/resampling files..");
                    for (int j = 0; j < customSoundCount; j++)
                    {
                        try
                        {
                            ConvertToWav(customSoundNames[j], path + @"temp_sounds" + customSoundNames[j].Remove(0, customSoundNames[j].LastIndexOf('\\')).Replace(".mp3", ".wav").Replace(".ogg", ".wav"));
                        }
                        catch (Exception ex) { UpdateUIReplaceTab("", "Failed to convert " + customSoundNames[j].Remove(0, customSoundNames[j].LastIndexOf('\\') + 1) + "! " + ex.Message); }
                        UpdateUIReplaceTab(customSoundNames[j] + " " + (j + 1) + "/" + customSoundCount, "");
                    }

                    UpdateUI("", "Replacing Sounds..");
                    if (graceCheckBox.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"Sounds\grace", "*.wav", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            UpdateUIReplaceTab(names[i], "");
                            i++;
                        }
                    if (tripCheckBox.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"Sounds\trip", "*.wav", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            UpdateUIReplaceTab(names[i], "");
                            i++;
                        }
                    if (globalCheckBox.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"Sounds\global", "*.wav", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            UpdateUIReplaceTab(names[i], "");
                            i++;
                        }
                    int tempCustomSoundCount = 0; //!!!!!!
                    foreach (string file in Directory.EnumerateFiles(path + @"temp_sounds", "*.wav", SearchOption.AllDirectories))
                    {
                        tempcustomSoundNames[tempCustomSoundCount] = file;
                        UpdateUIReplaceTab(names[i], "");
                        tempCustomSoundCount++;
                    }
                    UpdateUIReplaceTab("", "Copying resampled Files..");
                    Random random = new Random();
                    int rndnumber;
                    string[] rndnames = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        rndnumber = random.Next(tempCustomSoundCount);
                        if (chanceCheckBox.Checked)
                            if (random.Next(100) < Convert.ToInt32(chanceTextBox.Text))
                            {
                                File.Copy(tempcustomSoundNames[rndnumber], names[j], true);
                                randomlyCopied++;
                            }
                            else;
                        else File.Copy(tempcustomSoundNames[rndnumber], names[j], true);
                        UpdateUIReplaceTab(tempcustomSoundNames[rndnumber].Remove(0, (tempcustomSoundNames[rndnumber].LastIndexOf('\\') + 1)) + " to " + names[j] + " " + (j + 1) + "/" + i, "");
                    }
                    Directory.Delete(path + @"temp_sounds", true);
                    if (chanceCheckBox.Checked)
                        UpdateUIReplaceTab(" ", "Succesfully replaced the sound files. " + randomlyCopied + " out of " + i + " sound files has been replaced.");
                    else
                        UpdateUIReplaceTab(" ", "Succesfully replaced the sound files.");
                    //UpdateUIReplaceTab(" ", "Tip: If you want to replace with the same custom sound files, use randomize instead, it will be way faster!");
                }
                catch (Exception ex)
                {
                    UpdateUIReplaceTab(" ", "Error: " + ex.Message);
                }
            });


        }
        public void UpdateUIReplaceTab(string progress, string textboxmessage)
        {
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                if (progress != "") label1.Text = progress; if (textboxmessage != "") replaceLogTextBox.AppendText(textboxmessage + Environment.NewLine);
            }), "");
        }
        private void ConvertToWav(string inPath, string outPath)
        {
            if (inPath.Contains(".ogg"))
            {
                using (var vorbisStream = new VorbisWaveReader(inPath))
                {
                    var outFormat = new WaveFormat(22050, 1);
                    using (var resampler = new MediaFoundationResampler(vorbisStream, outFormat))
                    {
                        WaveFileWriter.CreateWaveFile(outPath, resampler);
                        //wav length    
                        //UpdateUIReplaceTab("",_outPath_.Remove(0, _outPath_.LastIndexOf("\\"))+" "+Convert.ToString(reader.Length));
                    }

                }
            }
            else
                using (var reader = new MediaFoundationReader(inPath))
                {
                    var outFormat = new WaveFormat(22050, 1);
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        WaveFileWriter.CreateWaveFile(outPath, resampler);
                        //wav length    
                        //UpdateUIReplaceTab("",_outPath_.Remove(0, _outPath_.LastIndexOf("\\"))+" "+Convert.ToString(reader.Length));
                    }
                }
        }

        private void chanceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (chanceCheckBox.Checked)
                chanceTextBox.Enabled = true;
            else
            {
                chanceTextBox.Enabled = false; chanceTextBox.Text = "100";
            }
        }

        private void chanceTextBox_KeyPress(object sender, KeyPressEventArgs e) //numbers only
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void texturesTextBox_CheckedChanged(object sender, EventArgs e)
        {
            if (texturesTextBox.Checked) lettersTextureIntactCheckBox.Enabled = true;
            else lettersTextureIntactCheckBox.Enabled = false;
        }


        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e) //check if path is empty when trying to switch tabs
        {
            if (pathTextbox.Text == "")
            {
                MessageBox.Show("Please point me to your game path first", "Hey", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }

        }
        void checkAdvancedSettings() //checking what advanced setting has been enabled before
        {
            var fileCompare = File.ReadAllText(path.Replace("sources", "classes") + "main.class");
            var fileCompare2 = File.ReadAllText(@"files\main_orig.class");
            if (fileCompare == fileCompare2)
                dramaManagerCheckBox.Checked = false;
            else
                dramaManagerCheckBox.Checked = true;

            fileCompare = File.ReadAllText(path.Replace(@"sources\facade", @"classes\facade\util") + "StringUtil.class");
            fileCompare2 = File.ReadAllText(@"files\StringUtil_orig.class");
            if (fileCompare == fileCompare2)
                AICheckBox.Checked = false;
            else
                AICheckBox.Checked = true;

            fileCompare = File.ReadAllText(path.Replace(@"sources\facade", @"classes\nlu\reaction") + "ReactionDecider.class");
            fileCompare2 = File.ReadAllText(@"files\ReactionDecider_orig.class");
            if (fileCompare == fileCompare2)
                decompressedCheckBox.Checked = false;
            else
                decompressedCheckBox.Checked = true;

            if (FindBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 }) == -1)
            {
                javaDebugCheckBox.Checked = false;
                launchButton.Enabled = false;
            }
            else
            {
                javaDebugCheckBox.Checked = true;
                launchButton.Enabled = true;
            }

        }

        private void dramaManagerCheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (path.Contains(@"sources"))
                    if (dramaManagerCheckBox.Checked)
                    {
                        MessageBox.Show("These can take some time to learn, and you will experience a lot of freezes, crashes experimenting with these options!", "Dude watch out", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        File.Copy(@"files\main.class", path.Replace("sources", "classes") + "main.class", true);
                    }
                    else
                        File.Copy(@"files\main_orig.class", path.Replace("sources", "classes") + "main.class", true);

                else
                {
                    MessageBox.Show("Not the right path");
                    dramaManagerCheckBox.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Oh no");
                dramaManagerCheckBox.Checked = !dramaManagerCheckBox.Checked;
            }

        }

        private void AICheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (path.Contains(@"sources"))
                    if (AICheckBox.Checked)
                        File.Copy(@"files\StringUtil.class", path.Replace(@"sources\facade", @"classes\facade\util") + "StringUtil.class", true);
                    else
                        File.Copy(@"files\StringUtil_orig.class", path.Replace(@"sources\facade", @"classes\facade\util") + "StringUtil.class", true);
                else
                {
                    MessageBox.Show("Not the right path");
                    AICheckBox.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Oh no");
                AICheckBox.Checked = !AICheckBox.Checked;
            }
        }

        private void decompressButton_Click(object sender, EventArgs e)
        {
            try
            {
                Decompress(path + "nlu/reaction/ReactionUtilities.bin");
                Decompress(path + "nlu/reaction/Proposer_GlobalMixIn.bin");
                Decompress(path + "nlu/reaction/Proposer_DuringMixin.bin");
                Decompress(path + "nlu/reaction/Proposer_DuringMixin_old.bin");
                Decompress(path + "nlu/reaction/Proposer_DuringBeatMixin.bin");
                Decompress(path + "nlu/reaction/Proposer_DuringTxnOut.bin");
                Decompress(path + "nlu/reaction/Proposer_IgnoreAllButRecovery.bin");
                Decompress(path + "nlu/reaction/Proposer_IgnoreMost.bin");
                Decompress(path + "nlu/reaction/Proposer_IgnoreThanks.bin");
                Decompress(path + "nlu/reaction/Proposer_TGreetsP.bin");
                Decompress(path + "nlu/reaction/Proposer_TGreetsP_TxnOut.bin");
                Decompress(path + "nlu/reaction/Proposer_GGreetsP.bin");
                Decompress(path + "nlu/reaction/Proposer_GGreetsPKiss.bin");
                Decompress(path + "nlu/reaction/Proposer_ExplDatAnniv.bin");
                Decompress(path + "nlu/reaction/Proposer_AA.bin");
                Decompress(path + "nlu/reaction/Proposer_AA_postTellMeMore.bin");
                Decompress(path + "nlu/reaction/Proposer_RM_ItalyGuessingGame.bin");
                Decompress(path + "nlu/reaction/Proposer_RM_PlayerNotAtPicture.bin");
                Decompress(path + "nlu/reaction/Proposer_FAskDrink.bin");
                Decompress(path + "nlu/reaction/Proposer_PhoneCall.bin");
                Decompress(path + "nlu/reaction/Proposer_TxnT1ToT2.bin");
                Decompress(path + "nlu/reaction/Proposer_OneOnOneAffChr.bin");
                Decompress(path + "nlu/reaction/Proposer_OneOnOneAffChr_xtra.bin");
                Decompress(path + "nlu/reaction/Proposer_OneOnOneNonAffChr.bin");
                Decompress(path + "nlu/reaction/Proposer_NonAffChrGReturns.bin");
                Decompress(path + "nlu/reaction/Proposer_NonAffChrGReturns_xtra.bin");
                Decompress(path + "nlu/reaction/Proposer_NonAffChrTReturns.bin");
                Decompress(path + "nlu/reaction/Proposer_RomanticProposal.bin");
                Decompress(path + "nlu/reaction/Proposer_BigQuestion.bin");
                Decompress(path + "nlu/reaction/Proposer_CrisisP1.bin");
                Decompress(path + "nlu/reaction/Proposer_C2TGGlue.bin");
                Decompress(path + "nlu/reaction/Proposer_TherapyGameP2.bin");
                Decompress(path + "nlu/reaction/Proposer_RevelationsP2.bin");
                Decompress(path + "nlu/reaction/Proposer_Ending.bin");
                Decompress(path + "nlu/reaction/ContextPriorityMap_GlobalTrumpsBeat.bin");
                Decompress(path + "nlu/reaction/ContextPriorityMap_GlobalTrumpsBeat_obj.bin");
                Decompress(path + "nlu/reaction/ContextPriorityMap_GlobalTrumpsBeat_veryHighPri.bin");
                Decompress(path + "nlu/reaction/Selector_Standard.bin");
                Decompress(path + "general.map");
                Decompress(path + "general.rul");
                MessageBox.Show("Done, you can find the files with the .jess file extension in the same directory as the source files. They're all written in Jess rule engine language for Java.", "Alright");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Damn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Decompress(string pathDecompile) //Yes, it's just decompressing, but if the devs called them bin files, why can't I call it decompiling? :)
        {
            using (FileStream compressedFileStream = File.Open(pathDecompile, FileMode.Open))
            using (FileStream outputFileStream = File.Create(pathDecompile + ".jess"))
            using (var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                decompressor.CopyTo(outputFileStream);

        }
        /*  Doesn't work, the game will crash. When recompressing the .bin.jess files with this method, the header and the bottom is slightly different than the original.

            public void Compress() 
            {
               using (FileStream originalFileStream = File.Open(pathcompile, FileMode.Open))
                   using (FileStream compressedFileStream = File.Create(pathcompile + ".rul"))
               using( var compressor = new DeflateStream(compressedFileStream,CompressionLevel.Fastest))
               originalFileStream.CopyTo(compressor);
            }*/

        private void decompressedCheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (path.Contains(@"sources"))
                    if (decompressedCheckBox.Checked)
                    {
                        File.Copy(@"files\Main$4NluTemplateThread.class", path.Replace("sources", "classes") + "Main$4NluTemplateThread.class", true);
                        File.Copy(@"files\NLUMain.class", path.Replace(@"sources\facade", @"classes\TemplateCompiler") + "NLUMain.class", true);
                        File.Copy(@"files\ReactionDecider.class", path.Replace(@"sources\facade", @"classes\nlu\reaction") + "ReactionDecider.class", true);
                        MessageBox.Show("The game will use the decompiled files with the .jess extension, don't change the file names! If you didn't click the disassemble button yet, the game won't work!", "A word of warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        File.Copy(@"files\Main$4NluTemplateThread_orig.class", path.Replace("sources", "classes") + "Main$4NluTemplateThread.class", true);
                        File.Copy(@"files\NLUMain_orig.class", path.Replace(@"sources\facade", @"classes\TemplateCompiler") + "NLUMain.class", true);
                        File.Copy(@"files\ReactionDecider_orig.class", path.Replace(@"sources\facade", @"classes\nlu\reaction") + "ReactionDecider.class", true);
                    }
                else { MessageBox.Show("Not the right path"); decompressedCheckBox.Checked = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Oh no");
                decompressedCheckBox.Checked = !decompressedCheckBox.Checked;
            }
        }

        private void pathTextbox_KeyPress(object sender, KeyPressEventArgs e) =>
            MessageBox.Show("Please use the browse button", "Stop");

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                File.WriteAllText(path.Replace(@"util\sources\facade\", @"util\j2re1.4.2_06\bin\javb.bat"), "@echo off"); // the game will launch this empty bat file instead of java.exe, the program will launch the java.exe seperately with a batch file, so the java backend won't be hidden
                string linesToWrite = "@echo off" + Environment.NewLine;
                linesToWrite += path[0] + ":" + Environment.NewLine;
                linesToWrite += @"cd " + "\"" + path + "\"" + Environment.NewLine;
                linesToWrite += "start animEngineStarter.exe" + Environment.NewLine;
                linesToWrite += "timeout 3" + Environment.NewLine;
                linesToWrite += "\"" + path.Replace(@"util\sources\facade\", @"util\j2re1.4.2_06\bin\java.exe").Replace(@"java.exe\", "java.exe") + "\" -Xms40m -Xmx250m -XX:NewSize=25m -enableassertions -Xfuture -Xincgc -showversion -classpath ../../classes;../../classes/jess.jar facade.Main" + Environment.NewLine;
                linesToWrite += "pause";
                //it's hard to read with all the replacing I know, but you will see the result at Facade\util\j2re1.4.2_06\bin\java.bat


                File.WriteAllText(path.Replace(@"util\sources\facade\", @"util\j2re1.4.2_06\bin\java.bat"), linesToWrite);
                Process.Start(path.Replace(@"util\sources\facade\", @"util\j2re1.4.2_06\bin\java.bat"));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't replace. Error message: " + ex.Message + "\nPlease restore the animEngineDLL.dll manually from Facade\\util\\sources\\facade\\Backup if problem persists", "Oh no", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;

            int index = FindBytes(src, search);
            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];
                //before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);
                //repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                //rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }


            return dst;
        }
        public int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }

        private void javaDebugCheckBox_Click(object sender, EventArgs e)
        {
            if (javaDebugCheckBox.Checked)
                try
                {
                    if (!File.Exists(path + @"Backup\animEngineDLL.dll"))
                    {
                        if (!Directory.Exists(path + "Backup")) Directory.CreateDirectory(path + "Backup");
                        File.Copy(path + @"animEngineDLL.dll", path + @"Backup\animEngineDLL.dll");
                        MessageBox.Show(@"Backup generated for animEngineDLL.dll at Facade\util\sources\facade\Backup just in case something goes wrong");
                    }
                    File.WriteAllBytes(path + "animEngineDLL.dll", ReplaceBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x61, 0x2e, 0x65, 0x78, 0x65 }, new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 })); // basically hex editing out "java.exe" with "javb.bat" in the DLL
                    launchButton.Enabled = true;
                    MessageBox.Show("Don't forget to disable this, otherwise the game will just load forever when you're not using the Launch button!", "Don't forget!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't replace. Error message: " + ex.Message + "\nPlease try to restore the animEngineDLL.dll manually from Facade\\util\\sources\\facade\\Backup if problem persists", "Is the game still running?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    javaDebugCheckBox.Checked = false;
                    launchButton.Enabled = false;
                }
            else
            {
                try
                {
                    launchButton.Enabled = false;
                    File.WriteAllBytes(path + "animEngineDLL.dll", ReplaceBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 }, new byte[] { 0x6a, 0x61, 0x76, 0x61, 0x2e, 0x65, 0x78, 0x65, }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't replace. Error message: " + ex.Message + "\nPlease try to restore the animEngineDLL.dll manually from Facade\\util\\sources\\facade\\Backup if problem persists", "Is the game still running?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    javaDebugCheckBox.Checked = true;
                    launchButton.Enabled = true;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("I used JD-GUI for decompiling, that gave me the best results. Unfortunately I haven't found any way to recompile them, so that's for reading only. I used bytecode editing on the compiled files instad, I used a program called Recaf for that. To decompile all the class files, open up Facade\\util\\classes\\facade\\Main.class with JD-GUI.", "Info");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(@"Open up this program's GitHub repository?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Process.Start(@"https://github.com/G4B33/facade_editor");
        }




    }
}
