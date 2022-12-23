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
        SynchronizationContext synchronizationContext; //this is needed for updating the UI while doing tasks on another thread
        string version = "1.0.1a";
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            initializeAndReadSettings();
            if (IsCurrentUserInAdminGroup())
                Text = "Façade editor (Admin rights) " + version;
            else 
                Text = "Façade editor " + version;

        }
        string[] names = new string[100000]; //used for storing path for the files to randomize, yes I will change it to list or something else later
        string path = @""; //path to game
        int i = 0; //used for a lot of things, sorry will fix this sometime

        async void initializeAndReadSettings()
        {
            if (File.Exists("settings.cfg"))
            {
                path = File.ReadAllText("settings.cfg");
                pathTextbox.Text = path;
                try
                {
                    checkProgramFilesFolder();
                    checkSettings();
                    await GenerateBackup();
                }
                catch
                {
                    MessageBox.Show("The path might be wrong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        async Task removeReadOnly() //I have to remove the read-only attributes, otherwise the program sometimes can't access them
        {
            try
            {
                await Task.Run(() =>
                {
                    var di = new DirectoryInfo(path);
                    foreach (var file in di.GetFiles("*.txt", SearchOption.AllDirectories))
                        file.Attributes &= ~FileAttributes.ReadOnly;
                });
            }
            catch { }
        }
        bool IsCurrentUserInAdminGroup() //check if run as admin
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        bool isTheGameRunning()
        {
            Process[] proc = Process.GetProcessesByName("animEngineStarter");
            if (proc.Length == 0)
                return false; // no
            else
                return true; // yes
        }


        public async void randomizeButton_Click(object sender, EventArgs e)
        {
            if (!isTheGameRunning())
            {
                disableButtonsAndStuff();
                if (soundsCheckBox.Checked || texturesCheckBox.Checked || cursorsCheckBox.Checked || animationsCheckBox.Checked || subtitlesCheckBox.Checked)
                {
                    removeTempFiles();
                    if (soundsCheckBox.Checked) await randomizeSounds();
                    if (texturesCheckBox.Checked) await randomizeTextures();
                    if (cursorsCheckBox.Checked) await randomizeCursors();
                    if (animationsCheckBox.Checked && animationsHardCorruptionCheckBox.Checked)
                        await randomizeAnimationsHard();
                    if (animationsCheckBox.Checked && !animationsHardCorruptionCheckBox.Checked)
                    {
                        await randomizeAnimations("grace");
                        await randomizeAnimations("trip");
                    }
                    if (subtitlesCheckBox.Checked)
                    {
                        await randomizeSubtitles("grace");
                        await randomizeSubtitles("trip");
                    }


                }
                else MessageBox.Show("You need to select what you want to randomize with the checkboxes first.", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Information);
                enableButtonsAndStuff();
            }
            else
                MessageBox.Show("The game is running, please close it first","Wait",MessageBoxButtons.OK,MessageBoxIcon.Stop);
        }

        void removeTempFiles() //if the program shuts down while working, this removes any temp files
        {
            try
            {
                if (Directory.Exists(path + "Sounds_r")) Directory.Delete(path + "Sounds_r", true);
                if (Directory.Exists(path + "textures_r")) Directory.Delete(path + "textures_r", true);
                if (Directory.Exists(path + "cursors_r")) Directory.Delete(path + "cursors_r", true);
                if (Directory.Exists(path + "animation_r")) Directory.Delete(path + "animation_r", true);
                if (Directory.Exists(path + "temp_sounds")) Directory.Delete(path + "temp_sounds", true);
            }
            catch { }
        }
        private async void restoreButton_Click(object sender, EventArgs e)
        {
            if (!isTheGameRunning())
            {
                disableButtonsAndStuff();
                if (soundsCheckBox.Checked || texturesCheckBox.Checked || cursorsCheckBox.Checked || animationsCheckBox.Checked || subtitlesCheckBox.Checked)
                    await Restore();
                else
                    MessageBox.Show("You need to select what you want to restore with the checkboxes first.", "Wait");
                enableButtonsAndStuff();
            }
            else
                MessageBox.Show("The game is running, please close it first", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                        CopyFilesRecursively(path + @"Backup\Sounds", path + @"Sounds", "restore");
                    }
                    if (texturesCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring textures..");
                        try
                        {
                            var di = new DirectoryInfo(path + @"textures");
                            foreach (var file in di.GetFiles("*", SearchOption.AllDirectories)) //I need to remove the read-only attributes , otherwise the program sometimes can't access them
                                file.Attributes &= ~FileAttributes.ReadOnly;
                            var di2 = new DirectoryInfo(path + @"Backup\textures");
                            foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                                file.Attributes &= ~FileAttributes.ReadOnly;
                        }
                        catch { }

                        CopyFilesRecursively(path + @"Backup\textures", path + @"textures", "restore");
                    }
                    if (cursorsCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring cursors..");
                        CopyFilesRecursively(path + @"Backup\cursors", path + @"cursors", "restore");
                    }
                    if (animationsCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring animations..");
                        CopyFilesRecursively(path + @"Backup\animation", path + @"animation", "restore");
                    }
                    if (subtitlesCheckBox.Checked)
                    {
                        UpdateUI("", "Restoring subtitles..");
                        File.Copy(path + @"Backup\graceScript.sbinary", path + @"animation\grace\graceScript.sbinary", true);
                        File.Copy(path + @"Backup\tripScript.sbinary", path + @"animation\trip\tripScript.sbinary", true);
                    }
                    UpdateUI(" ", "Files restored.");
                }
                catch (Exception ex)
                {
                    UpdateUI("", "Error restoring: " + ex.Message);
                }
            });


        }
        void disableButtonsAndStuff()
        {
            randomizeButton.Enabled = false;
            replaceButton.Enabled = false;
            browseButton.Enabled = false;
            replaceBrowseButton.Enabled = false;
            clearListButton.Enabled = false;
            restoreButton.Enabled = false;
            launchButton.Enabled = false;
            soundsCheckBox.Enabled = false;
            texturesCheckBox.Enabled = false;
            cursorsCheckBox.Enabled = false;
            animationsCheckBox.Enabled = false;
            dontUseFilesFromBackupRadioButton.Enabled = false;
            useBackupFilesRadioButton.Enabled = false;
            lettersTextureIntactCheckBox.Enabled = false;
            animationsHardCorruptionCheckBox.Enabled = false;
            graceCheckBox.Enabled = false;
            tripCheckBox.Enabled = false;
            globalCheckBox.Enabled = false;
            chanceCheckBox.Enabled = false;
            chanceTextBox.Enabled = false;
        }
        void enableButtonsAndStuff()
        {
            randomizeButton.Enabled = true;
            replaceButton.Enabled = true;
            browseButton.Enabled = true;
            replaceBrowseButton.Enabled = true;
            clearListButton.Enabled = true;
            if (Directory.Exists(path + "Backup"))
                restoreButton.Enabled = true;
            if (javaDebugCheckBox.Checked)
                launchButton.Enabled = true;
            soundsCheckBox.Enabled = true;
            texturesCheckBox.Enabled = true;
            cursorsCheckBox.Enabled = true;
            animationsCheckBox.Enabled = true;
            dontUseFilesFromBackupRadioButton.Enabled = true;
            if (Directory.Exists(path + "Backup"))
            useBackupFilesRadioButton.Enabled = true;
            if (texturesCheckBox.Checked)
                lettersTextureIntactCheckBox.Enabled = true;
            if (animationsCheckBox.Checked)
                animationsHardCorruptionCheckBox.Enabled = true;
            graceCheckBox.Enabled = true;
            tripCheckBox.Enabled = true;
            globalCheckBox.Enabled = true;
            chanceCheckBox.Enabled = true;
            if (chanceCheckBox.Checked)
                chanceTextBox.Enabled = true;
        }
        async Task GenerateBackup()
        {
            UpdateUI("", "Checking for backup..");
            try
            {
                if (!Directory.Exists(path + @"Backup\Sounds"))
                {
                    UpdateUI("", "Backup not found");
                    if (MessageBox.Show(@"Do you want to generate a backup? You can use those original files to restore later. The backup will be saved at Facade\util\sources\facade\Backup. It is very recommended to make one, especially if the program shuts down unexpectedly while working.", "Backup is very recommended", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        disableButtonsAndStuff();
                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(path + @"Backup\animation")) Directory.CreateDirectory(path + @"Backup\animation");
                            CopyFilesRecursively(path + @"animation", path + @"Backup\animation", "backup");
                            if (!Directory.Exists(path + @"Backup\cursors")) Directory.CreateDirectory(path + @"Backup\cursors");
                            CopyFilesRecursively(path + @"cursors", path + @"Backup\cursors", "backup");
                            if (!Directory.Exists(path + @"Backup\Sounds")) Directory.CreateDirectory(path + @"Backup\Sounds");
                            CopyFilesRecursively(path + @"Sounds", path + @"Backup\Sounds", "backup");
                            if (!Directory.Exists(path + @"Backup\textures")) Directory.CreateDirectory(path + @"Backup\textures");
                            CopyFilesRecursively(path + @"textures", path + @"Backup\textures", "backup");
                        });
                        UpdateUI(" ", "Backup generated.");
                        useBackupFilesRadioButton.Enabled = true;
                        enableButtonsAndStuff();
                    }

                }
                else
                {
                    UpdateUI("", "Backup found.");
                    useBackupFilesRadioButton.Enabled = true;
                    enableButtonsAndStuff();
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
        void CopyFilesRecursively(string sourcePath, string targetPath, string restoreOrBackup) 
        {
            //create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            //copy all the files
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);

                if (restoreOrBackup=="restore")
                    UpdateUI("Restoring " + newPath + "..", "");
                if (restoreOrBackup == "backup")
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
                        CopyFilesRecursively(path + @"Sounds\mp3_1", path + @"Sounds_r\mp3_1", "copy");
                        CopyFilesRecursively(path + @"Sounds\mp3_2", path + @"Sounds_r\mp3_2", "copy");
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
        async Task randomizeAnimationsHard()
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

                    if (dontUseFilesFromBackupRadioButton.Checked)
                    {
                        if (!Directory.Exists(path + @"animation_r")) Directory.CreateDirectory(path + @"animation_r");
                        if (!Directory.Exists(path + @"animation_r\grace")) Directory.CreateDirectory(path + @"animation_r\grace");
                        if (!Directory.Exists(path + @"animation_r\trip")) Directory.CreateDirectory(path + @"animation_r\trip");
                        if (!File.Exists(path + @"animation_r\grace\grace.aindex")) File.Copy(path + @"animation\grace\grace.aindex", path + @"animation_r\grace\grace.aindex");
                        if (!File.Exists(path + @"animation_r\grace\graceScript.sbinary")) File.Copy(path + @"animation\grace\graceScript.sbinary", path + @"animation_r\grace\graceScript.sbinary");
                        if (!File.Exists(path + @"animation_r\trip\trip.aindex")) File.Copy(path + @"animation\trip\trip.aindex", path + @"animation_r\trip\trip.aindex");
                        if (!File.Exists(path + @"animation_r\trip\tripScript.sbinary")) File.Copy(path + @"animation\trip\tripScript.sbinary", path + @"animation_r\trip\tripScript.sbinary");
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

        }
        async Task randomizeAnimations(string who)
        {
            i = 0;
            UpdateUI("", "Randomizing "+ char.ToUpper(who[0]) + who.Substring(1) + "'s animations..");

            await Task.Run(() =>
            {
                try
                {
                    if (dontUseFilesFromBackupRadioButton.Checked)
                        foreach (string file in Directory.EnumerateFiles(path + @"animation\" + who, "*.abinary", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                    else
                        foreach (string file in Directory.EnumerateFiles(path + @"Backup\animation\"+who, "*.abinary", SearchOption.AllDirectories))
                        {
                            names[i] = file;
                            i++;
                        }
                    
                    if (dontUseFilesFromBackupRadioButton.Checked && who == "grace")
                    {
                        if (!Directory.Exists(path + @"animation_r")) Directory.CreateDirectory(path + @"animation_r");
                        if (!Directory.Exists(path + @"animation_r\grace")) Directory.CreateDirectory(path + @"animation_r\grace");
                        if (!File.Exists(path + @"animation_r\grace\grace.aindex")) File.Move(path + @"animation\grace\grace.aindex", path + @"animation_r\grace\grace.aindex");
                        if (!File.Exists(path + @"animation_r\grace\graceScript.sbinary")) File.Move(path + @"animation\grace\graceScript.sbinary", path + @"animation_r\grace\graceScript.sbinary");
                    }
                    if (dontUseFilesFromBackupRadioButton.Checked && who == "trip")
                    {
                        if (!Directory.Exists(path + @"animation_r")) Directory.CreateDirectory(path + @"animation_r");
                        if (!Directory.Exists(path + @"animation_r\trip")) Directory.CreateDirectory(path + @"animation_r\trip");
                        if (!File.Exists(path + @"animation_r\trip\trip.aindex")) File.Move(path + @"animation\trip\trip.aindex", path + @"animation_r\trip\trip.aindex");
                        if (!File.Exists(path + @"animation_r\trip\tripScript.sbinary")) File.Move(path + @"animation\trip\tripScript.sbinary", path + @"animation_r\trip\tripScript.sbinary");
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
                            rndnames[j] = names[rndnumber - 1].Replace("animation", "animation_r");
                            if (dontUseFilesFromBackupRadioButton.Checked)
                            {
                                rndnames[j] = names[rndnumber - 1].Replace(@"animation", "animation_r");
                                //File.Move(names[j], rndnames[j]); There's an issue with this sometimes, using copy instead for now..
                                File.Copy(names[j], rndnames[j],true);
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
                        if (who == "grace")
                            Directory.Move(path + @"animation\trip", path + @"animation_r\trip");
                        if (who == "trip")
                            Directory.Move(path + @"animation\grace", path + @"animation_r\grace");
                        Directory.Delete(path + @"animation", true);
                        Directory.Move(path + @"animation_r", path + @"animation");
                    }
                    UpdateUI(" ", "Animations randomized succesfully.");
                }
                catch (Exception ex)
                {
                    UpdateUI(" ", "Error randomizing animations: " + ex.Message+"\nUse restore, then try again. If the problem persists, try to use the \"Use original files from backup option.\"");
                }
            });

        }
        async Task randomizeSubtitles(string who)
        {
            UpdateUI("","Randomizing " + char.ToUpper(who[0]) + who.Substring(1) + "'s subtitles..");
            if (!Directory.Exists(path + "Backup"))
                Directory.CreateDirectory(path + "Backup");

            if (!File.Exists(path + @"Backup\" + who + "Script.sbinary"))
                File.Copy(path + @"animation\" + who + @"\" + who + "Script.sbinary", path + @"Backup\" + who + "Script.sbinary", true);
            
            await Task.Run(() =>
            {
                try
                {
                    if (who == "grace")
                        readGraceSubtitles();
                    if (who == "trip")
                        readTripSubtitles();
                    byte[] subFile = File.ReadAllBytes(path + @"Backup\" + who + "Script.sbinary");
                    string[] subtitles = new string[10000];
                    int[] indexes = new int[10000];
                    int counter = 0;
                    Random r = new Random();
                    foreach (string line in File.ReadLines(path + who + "SubnonumberIndex.txt"))
                    {
                        subtitles[counter] = line.Remove(line.IndexOf('='));
                        indexes[counter] = Convert.ToInt32(line.Remove(0, line.IndexOf('=') + 1));
                        counter++;
                    }
                    for (int i = 0; i < counter; i++)
                        try
                        {
                            subFile = ReplaceBytes(subFile, convert(Encoding.ASCII.GetBytes(subtitles[i])), convert(Encoding.ASCII.GetBytes(subtitles[r.Next(0, counter)])), indexes[i]);
                        }
                        catch (Exception e) { UpdateUI("", "error " + subtitles[i] + " " + e.Message); }
                    File.WriteAllBytes(path + @"animation\" + who + @"\" + who + @"Script.sbinary", subFile);
                    UpdateUI("", char.ToUpper(who[0]) + who.Substring(1) + "'s subtitles randomized succesfully.");
                }
                catch(Exception e)
                {
                    UpdateUI("","Error randomizing " + char.ToUpper(who[0]) + who.Substring(1) + "'s subtitles: " + e.Message);
                }
            });
            try
            {
                if (File.Exists(path + @"graceScript.txt"))File.Delete(path + @"graceScript.txt");
                if (File.Exists(path + @"tripScript.txt")) File.Delete(path + @"tripScript.txt");
            }
            catch { }
        }
         void readGraceSubtitles() // It's very, very jank, proceed with caution!
        {

            byte[] subFile = File.ReadAllBytes(path + @"Backup\graceScript.sbinary");
            byte[] letters = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x3f, 0x21, 0x2e, 0x2c, 0x28, 0x29, 0x20, 0x27, 0x2D };
            int index = 0;
            byte[] subs = new byte[subFile.Length];
            int[] startsFromIndex = new int[10000];
            startsFromIndex[0] = 8242;
            int startsFromIndexIndex = 1;

            for (int i = 0; i < subs.Length; i++)
            {
                for (int j = 0; j < letters.Length; j++)
                    if (subFile[i] == letters[j] && subFile[i - 1] != 0xFF)
                    {
                        subs[index] = subFile[i];
                        index++;
                    }
                if (subFile[i] == 0xFF && subFile[i + 1] == 0xFF && subFile[i + 2] == 0xFF && subFile[i + 3] == 0xFF)
                {
                    subs[index] = 0x0A;
                    index++;
                    startsFromIndex[startsFromIndexIndex] = i;
                    startsFromIndexIndex++;
                }
            }
            //asd1[i] = asd[FindBytes(asd, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, }, 0) + i];

            File.WriteAllBytes(path + @"graceScript.txt", subs);
            int counter = 0;
            string tobewritten = "";
            startsFromIndexIndex = 0;
            foreach (string line in File.ReadLines(path + @"graceScript.txt"))
            {

                if (counter % 2 == 1 || counter == 1724 || counter == 2238 || counter == 2750 || counter == 2752 || counter == 3264 || counter == 3266 || counter == 3776 || counter == 3778 || counter == 3780 || counter == 4290 || counter == 4292 || counter == 4294 || counter == 4802 || counter == 4804 || counter == 4806 || counter == 4808 || counter == 5316 || counter == 5318 || counter == 5320 || counter == 5322 || counter == 5828 || counter == 5830 || counter == 5832 || counter == 5834 || counter == 5836 || counter == 6342 || counter == 6344 || counter == 6346 || counter == 6348 || counter == 6350 || counter == 6856 || counter == 6858 || counter == 6860 || counter == 6862 || counter == 6864 || counter == 7368 || counter == 7370 || counter == 7372 || counter == 7374 || counter == 7376 || counter == 7378 || counter == 7880 || counter == 7882 || counter == 7884 || counter == 7886 || counter == 7888 || counter == 7890 || counter == 7892)
                {
                    //Console.WriteLine(line);
                    tobewritten += line + "=" + startsFromIndex[startsFromIndexIndex] + Environment.NewLine;

                }
                if (counter == 183 || counter == 696 || counter == 1209 || counter == 1722 || counter == 2235 || counter == 2748 || counter == 3262 || counter == 3774 || counter == 4287 || counter == 4800 || counter == 5313 || counter == 5826 || counter == 6339 || counter == 6853 || counter == 7365 || counter == 7878)
                {
                    counter++;
                }
                startsFromIndexIndex++;
                counter++;
            }
            //File.WriteAllText(path + @"animation\grace\graceSubnonumber.txt", tobewritten);
            File.WriteAllText(path + @"graceSubnonumberIndex.txt", tobewritten);
        }
        void readTripSubtitles() // It's very, very jank, proceed with caution!
        {
            byte[] subFile = File.ReadAllBytes(path + @"Backup\tripScript.sbinary");
            byte[] letters = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x3f, 0x21, 0x2e, 0x2c, 0x28, 0x29, 0x20, 0x27, 0x2D, 0x5F };
            int index = 0;
            byte[] subs = new byte[subFile.Length];
            int[] startsFromIndex = new int[10000];
            startsFromIndex[0] = 8242;
            int startsFromIndexIndex = 1;

            for (int i = 0; i < subs.Length; i++)
            {
                for (int j = 0; j < letters.Length; j++)
                    if (subFile[i] == letters[j] && subFile[i - 1] != 0xFF)
                    {
                        subs[index] = subFile[i];
                        index++;
                    }
                if (subFile[i] == 0xFF && subFile[i + 1] == 0xFF && subFile[i + 2] == 0xFF && subFile[i + 3] == 0xFF)
                {
                    subs[index] = 0x0A;
                    index++;
                    startsFromIndex[startsFromIndexIndex] = i;
                    startsFromIndexIndex++;
                }
            }
            File.WriteAllBytes(path + "tripScript.txt", subs);
            int counter = 0;
            string tobewritten = "";
            startsFromIndexIndex = 0;
            foreach (string line in File.ReadLines(path + "tripScript.txt"))
            {
                if (counter % 2 == 1 || counter == 2038 || counter == 2552 || counter == 3064 || counter == 3066 || counter == 3578 || counter == 3580 || counter == 4090 || counter == 4092 || counter == 4094 || counter == 4602 || counter == 4604 || counter == 4606 || counter == 4608 || counter == 5116 || counter == 5118 || counter == 5120 || counter == 5122 || counter == 5630 || counter == 5632 || counter == 5634 || counter == 5636 || counter == 6142 || counter == 6144 || counter == 6146 || counter == 6148 || counter == 6150 || counter == 6654 || counter == 6656 || counter == 6658 || counter == 6660 || counter == 6662 || counter == 6664 || counter == 7168 || counter == 7170 || counter == 7172 || counter == 7174 || counter == 7176 || counter == 7178 || counter == 7680 || counter == 7682 || counter == 7684 || counter == 7686 || counter == 7688 || counter == 7690 || counter == 7692 || counter == 8194 || counter == 8196 || counter == 8198 || counter == 8200 || counter == 8202 || counter == 8204 || counter == 8206 || counter == 8706 || counter == 8708 || counter == 8710 || counter == 8712 || counter == 8714 || counter == 8716 || counter == 8718 || counter == 8720)
                {
                    //Console.WriteLine(line);
                    tobewritten += line + "=" + startsFromIndex[startsFromIndexIndex] + Environment.NewLine;
                }
                if (counter == 497 || counter == 1010 || counter == 1523 || counter == 2036 || counter == 2549 || counter == 3062 || counter == 3575 || counter == 4088 || counter == 4601 || counter == 5114 || counter == 5628 || counter == 6140 || counter == 6653 || counter == 7166 || counter == 7679 || counter == 8192 || counter == 8705)
                {
                    counter++;
                    //Console.ReadKey();
                }
                counter++;
                startsFromIndexIndex++;
            }
            File.WriteAllText(path + @"tripSubnonumberIndex.txt", tobewritten);
        }
        static byte[] convert(byte[] source) 
        {
            byte[] replaced = new byte[source.Length * 4];
            for (int i = 0; i < source.Length; i++)
            {
                replaced[i * 4] = source[i];
                replaced[i * 4 + 1] = 0x00;
                replaced[i * 4 + 2] = 0x00;
                replaced[i * 4 + 3] = 0x00;
            }
            return replaced;
        }

        void checkProgramFilesFolder()
        {
            if (path.Contains("Program Files") && !IsCurrentUserInAdminGroup())
            {
                MessageBox.Show("Program Files folder detected, I might not have permission to write there without administrator rights.", "A little warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                warningLabel.Text = "I might not have permission to write to that folder. Please try to run this program with administrator rights\nor move the game if the program fails to run properly.";
            }
            else warningLabel.Text = "";
        }

        private async void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                useBackupFilesRadioButton.Enabled = false;
                disableButtonsAndStuff();
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
                    checkProgramFilesFolder();
                    await GenerateBackup();
                }

                else { warningLabel.Text = "Façade folder not detected. Are you sure you've selected the right folder?"; path = folderBrowserDialog1.SelectedPath + @"\"; pathTextbox.Text = path; }
                enableButtonsAndStuff();
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
                    checkSettings();
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
            if (!isTheGameRunning())
            {
                disableButtonsAndStuff();
                if (customSoundCount != 0)
                {
                    removeTempFiles();
                    await replaceSounds();
                }
                else
                    UpdateUIReplaceTab("", "No custom sound files");
                enableButtonsAndStuff();
            }
            else
                MessageBox.Show("The game is running, please close it first", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
        void ConvertToWav(string inPath, string outPath)
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
                chanceTextBox.Enabled = false;
                chanceTextBox.Text = "100";
            }
        }

        private void chanceTextBox_KeyPress(object sender, KeyPressEventArgs e) //numbers only
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void texturesTextBox_CheckedChanged(object sender, EventArgs e)
        {
            if (texturesCheckBox.Checked)
                lettersTextureIntactCheckBox.Enabled = true;
            else
                lettersTextureIntactCheckBox.Enabled = false;
        }


        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e) //check if path is empty when trying to switch tabs
        {
            if (pathTextbox.Text == "")
            {
                MessageBox.Show("Please point me to your game path first", "Hey", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }

        }
        void checkSettings() //checking what setting has been enabled before
        {
            removeReadOnly();
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

            if (FindBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 },0) == -1)
            {
                javaDebugCheckBox.Checked = false;
                launchButton.Enabled = false;
            }
            else
            {
                javaDebugCheckBox.Checked = true;
                launchButton.Enabled = true;
            }

            fileCompare = File.ReadAllText(path + @"nlu\reaction\Proposer_GlobalMixIn.bin");
            fileCompare2 = File.ReadAllText(@"files\Proposer_GlobalMixIn_orig.bin");
            if (fileCompare == fileCompare2)
                godModeCheckbox.Checked = false;
            else
                godModeCheckbox.Checked = true;

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

        private void pathTextbox_KeyPress(object sender, KeyPressEventArgs e) => MessageBox.Show("Please use the browse button", "Stop");

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


        static int FindBytes(byte[] src, byte[] find, int startindex)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = startindex; i < src.Length; i++)
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
        static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl, int startIndex)
        {
            try
            {
                byte[] dst = null;

                int index = FindBytes(src, search, startIndex);
                if (index >= 0)
                {
                    dst = new byte[src.Length - search.Length + repl.Length];
                    // before found array
                    Buffer.BlockCopy(src, 0, dst, 0, index);
                    // repl copy
                    Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                    // rest of src array
                    Buffer.BlockCopy(
                        src,
                        index + search.Length,
                        dst,
                        index + repl.Length,
                        src.Length - (index + search.Length));
                    return dst;
                }
                else return src;



            }
            catch { return src; }
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
                    File.WriteAllBytes(path + "animEngineDLL.dll", ReplaceBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x61, 0x2e, 0x65, 0x78, 0x65 }, new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 },0)); // basically hex editing out "java.exe" with "javb.bat" in the DLL
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
                    File.WriteAllBytes(path + "animEngineDLL.dll", ReplaceBytes(File.ReadAllBytes(path + "animEngineDLL.dll"), new byte[] { 0x6a, 0x61, 0x76, 0x62, 0x2e, 0x62, 0x61, 0x74 }, new byte[] { 0x6a, 0x61, 0x76, 0x61, 0x2e, 0x65, 0x78, 0x65, },0));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't replace. Error message: " + ex.Message + "\nPlease try to restore the animEngineDLL.dll manually from Facade\\util\\sources\\facade\\Backup if problem persists", "Is the game still running?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    javaDebugCheckBox.Checked = true;
                    launchButton.Enabled = true;
                }
            }
        }

        private void animationsCheckBox_Click(object sender, EventArgs e)
        {
            animationsHardCorruptionCheckBox.Enabled = !animationsHardCorruptionCheckBox.Enabled;
        }

        private void animationsHardCorruptionCheckBox_Click(object sender, EventArgs e)
        {
            if (animationsHardCorruptionCheckBox.Checked)
            MessageBox.Show("It is recommended to leave this off, sometimes you get funnier results, but most of the time Grace's and/or Trip's sprite just disappear","Hey",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        }


        private void godModeCheckbox_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Yes;
            if (File.Exists(path + @"nlu\reaction\Proposer_GGreetsP.bin.jess") || File.Exists(path + @"nlu\reaction\Proposer_GlobalMixIn.bin.jess") || File.Exists(path + @"nlu\reaction\Proposer_TGreetsP.bin.jess"))
                dialogResult = MessageBox.Show("Warning! This will overwrite the Proposer_CrisisP1.bin.jess, Proposer_GGreetsP.bin.jess, Proposer_GlobalMixIn.bin.jess, Proposer_TGreetsP.bin.jess, Proposer_TherapyGameP2.bin.jess files found in the nlu\\reaction folder! If you have modified them, make a backup first. \n\n\nIf you didn't modify them, or don't know what i'm talking about, just press yes to continue.", "Woah", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                    try
                    {
                                if (path.Contains(@"sources"))
                                {
                                    if (godModeCheckbox.Checked)
                                    {
                                        File.Copy(@"files\Proposer_GGreetsP.bin", path + @"nlu\reaction\Proposer_GGreetsP.bin", true);
                                        File.Copy(@"files\Proposer_GlobalMixIn.bin", path + @"nlu\reaction\Proposer_GlobalMixIn.bin", true);
                                        File.Copy(@"files\Proposer_TGreetsP.bin", path + @"nlu\reaction\Proposer_TGreetsP.bin", true);
                                        File.Copy(@"files\Proposer_CrisisP1.bin", path + @"nlu\reaction\Proposer_CrisisP1.bin", true);
                                        File.Copy(@"files\Proposer_TherapyGameP2.bin", path + @"nlu\reaction\Proposer_TherapyGameP2.bin", true);
                                    }
                                    else
                                    {
                                        File.Copy(@"files\Proposer_GGreetsP_orig.bin", path + @"nlu\reaction\Proposer_GGreetsP.bin", true);
                                        File.Copy(@"files\Proposer_GlobalMixIn_orig.bin", path + @"nlu\reaction\Proposer_GlobalMixIn.bin", true);
                                        File.Copy(@"files\Proposer_TGreetsP_orig.bin", path + @"nlu\reaction\Proposer_TGreetsP.bin", true);
                                        File.Copy(@"files\Proposer_CrisisP1_orig.bin", path + @"nlu\reaction\Proposer_CrisisP1.bin", true);
                                        File.Copy(@"files\Proposer_TherapyGameP2_orig.bin", path + @"nlu\reaction\Proposer_TherapyGameP2.bin", true);
                                    }
                                    if (decompressedCheckBox.Checked || File.Exists(path + @"nlu\reaction\Proposer_GGreetsP.bin.jess") || File.Exists(path + @"nlu\reaction\Proposer_GlobalMixIn.bin.jess") || File.Exists(path + @"nlu\reaction\Proposer_TGreetsP.bin.jess"))
                                    {
                                        Decompress(path + @"nlu\reaction\Proposer_GGreetsP.bin");
                                        Decompress(path + @"nlu\reaction\Proposer_GlobalMixIn.bin");
                                        Decompress(path + @"nlu\reaction\Proposer_TGreetsP.bin");
                                        Decompress(path + @"nlu\reaction\Proposer_TherapyGameP2.bin");
                                        Decompress(path + @"nlu\reaction\Proposer_CrisisP1.bin");
                                    }
                                }
                                else { MessageBox.Show("Not the right path"); godModeCheckbox.Checked = false; }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error:" + ex.Message, "Oh no");
                        godModeCheckbox.Checked = !godModeCheckbox.Checked;
                    }
                else godModeCheckbox.Checked = !godModeCheckbox.Checked;
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
