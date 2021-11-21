using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Management;

//30018729
//Kin Lam Tang
//PROJECT
//Audio Player
namespace AudioPlayer
{
    public partial class musicPlayer : Form
    {
        public musicPlayer()
        {
            InitializeComponent();
        }

        //Dynamic Data Structure
        private LinkedList<string> SongList = new LinkedList<string>();
        private void DisplaySongTitle(string CurrentSong)
        {
            textBoxSong.Clear();
            FileInfo fi = new FileInfo(CurrentSong);
            string song = fi.Name;
            textBoxSong.Text = song;
        }

        public string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{ Path.DirectorySeparatorChar }{ rel }";
            }
            return rel;
        }

        //Add song
        private void buttonAdd_Click(object sender, EventArgs e)
        {

            openFileDialog1 = new OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int total = SongList.Count;             
                List<string> arr = new List<string>();
                foreach (string file in openFileDialog1.FileNames)
                {
                    arr.Add(file);
                }
                //heapSort(sortArr);
                String[] sortArr = arr.Cast<string>().ToArray();
                QuickSort(sortArr);
                foreach (string file in sortArr)
                {
                    
                    var process = Process.GetCurrentProcess();
                    string fullPath = process.MainModule.FileName;
                    var relativePath = GetRelativePath(fullPath, file);
                    try
                    {
                        int index = relativePath.LastIndexOf('\\');
                        string songName = relativePath.Substring(index + 1);
                        listBoxSong.Items.Add(songName);                        
                        String hashFileName = GetMD5HashFromFile(relativePath);
                        textBoxHashtext.Text = hashFileName;
                        SongList.AddLast(relativePath + "," + hashFileName);
                        String securityHash = relativePath + "," + hashFileName;
                    }
                    catch
                    {
                        MessageBox.Show("can't add a song");
                    }
                    //sort(songArr,file.Length);
                }
                string[] sortListBox = listBoxSong.Items.Cast<string>().ToArray();
                QuickSort(sortListBox);
                listBoxSong.Items.Clear();
                foreach (string file in sortListBox)
                {
                    listBoxSong.Items.Add(file);
                }
            }
        }

        //MD5
        //Third Party Library
        private static string GetMD5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
        //Start
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                string currentSong = SongList.First().Split(',')[0];
                textBoxHashtext.Text = SongList.First().Split(',')[1];
                DisplaySongTitle(currentSong);
                buttonNext.Enabled = true;
                buttonBack.Enabled = false;
                PlaySound(currentSong);
                int Index = 0;
                listBoxSong.SelectedIndex = Index;
            }
            catch
            {
                ErrorMsg.Text = "Cannot play First";
            }
        }

        //Next
        private void buttonNext_Click(object sender, EventArgs e)
        {
            string songPlaying = axWindowsMediaPlayer.URL;
            try
            {
                for (LinkedListNode<string> node = SongList.First; node != null; node = node.Next)
                {
                    int index = songPlaying.LastIndexOf('\\');
                    songPlaying = songPlaying.Substring(index + 1);
                    if (node.Value.Contains(songPlaying))
                    {
                        if (node.Next != null)
                        {
                            string nextSong = node.Next.Value.Split(',')[0];
                            textBoxHashtext.Text = node.Next.Value.Split(',')[1];
                            DisplaySongTitle(nextSong);
                            PlaySound(nextSong);
                            buttonBack.Enabled = true;
                            //buttonNext.Enabled = false;
                            int nextIndex = listBoxSong.SelectedIndex + 1;
                            listBoxSong.SelectedIndex = nextIndex;
                        }
                        else
                        {
                            buttonNext.Enabled = false;
                        }
                    }
                }
                /*if (SongList.Find(SongPlaying).Next.Value == SongList.Last())
                {
                    string lastSong = SongList.Last();
                    DisplaySongTitle(lastSong);
                    PlaySound(lastSong);
                    buttonBack.Enabled = true;
                    buttonNext.Enabled = false;
                }
                else
                {
                    string currentSong = SongList.Find(SongPlaying).Next.Value;
                    DisplaySongTitle(currentSong);
                    PlaySound(currentSong);
                    buttonBack.Enabled = true;
                }
                */
            }
            catch
            {
                ErrorMsg.Text = "Cannot play next";
            }
        }
        //Back
        private void buttonBack_Click(object sender, EventArgs e)
        {
            string songPlaying = axWindowsMediaPlayer.URL;
            try
            {
                for (LinkedListNode<string> node = SongList.First; node != null; node = node.Next)
                {
                    int index = songPlaying.LastIndexOf('\\');
                    songPlaying = songPlaying.Substring(index + 1);
                    if (node.Value.Contains(songPlaying))
                    {
                        if (node.Previous != null)
                        {
                            string previousSong = node.Previous.Value.Split(',')[0];
                            textBoxHashtext.Text = node.Previous.Value.Split(',')[1];
                            DisplaySongTitle(previousSong);
                            PlaySound(previousSong);
                            buttonNext.Enabled = true;
                            int backIndex = listBoxSong.SelectedIndex - 1;
                            listBoxSong.SelectedIndex = backIndex;
                            //buttonNext.Enabled = false;
                        }
                        else
                        {
                            buttonBack.Enabled = false;
                        }
                    }
                }
            }
            /*string SongPlaying = axWindowsMediaPlayer.URL;
            try
            {
                if (SongList.Find(SongPlaying).Previous.Value == SongList.First())
                {
                    string firstSong = SongList.First();
                    DisplaySongTitle(firstSong);
                    PlaySound(firstSong);
                    buttonBack.Enabled = false;
                    buttonNext.Enabled = true;
                }
                else
                {
                    string currentSong = SongList.Find(SongPlaying).Previous.Value;
                    DisplaySongTitle(currentSong);
                    PlaySound(currentSong);
                    buttonNext.Enabled = true;
                }
            }*/
            catch
            {
                ErrorMsg.Text = "Cannot play back";
            }
        }
        //End
        private void buttonEnd_Click(object sender, EventArgs e)
        {
            try
            {
                string currentSong = SongList.Last().Split(',')[0];
                textBoxHashtext.Text = SongList.Last().Split(',')[1];
                DisplaySongTitle(currentSong);
                buttonNext.Enabled = false;
                buttonBack.Enabled = true;
                PlaySound(currentSong);
                int total = SongList.Count;
                listBoxSong.SelectedIndex = total - 1;
            }
            catch
            {
                ErrorMsg.Text = "Cannot play End";
            }
        }
        private void PlaySound(string playsong)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), playsong);
                string absolute = Path.GetFullPath(playsong);
                axWindowsMediaPlayer.URL = absolute;
                axWindowsMediaPlayer.Ctlcontrols.play();
            }
            catch
            {
                ErrorMsg.Text = "song failed to play";
            }
        }


        private void listBoxSong_DoubleClick(object sender, EventArgs e)
        {
            int index;
            index = listBoxSong.SelectedIndex;
            string value = SongList.ElementAt(index).Split(',')[0];
            textBoxHashtext.Text = SongList.ElementAt(index).Split(',')[1];
            try
            {
                for (LinkedListNode<string> node = SongList.First; node != null; node = node.Next)
                {

                    if (node.Value.Contains(value))
                    {
                        if (node.Next != null && node.Previous != null)
                        {
                            DisplaySongTitle(value);
                            buttonNext.Enabled = true;
                            buttonBack.Enabled = true;
                            PlaySound(value);
                        }
                        if (node.Next != null && node.Previous == null)
                        {
                            DisplaySongTitle(value);
                            buttonNext.Enabled = true;
                            buttonBack.Enabled = false;
                            PlaySound(value);
                        }
                        if (node.Next == null && node.Previous != null)
                        {
                            DisplaySongTitle(value);
                            buttonNext.Enabled = false;
                            buttonBack.Enabled = true;
                            PlaySound(value);
                        }
                    }
                }
            }
            catch
            {
                ErrorMsg.Text = "Cannot play First";
            }
        }

        //Heap Sort
        private void heapSort<T>(T[] array) where T : IComparable<T>
        {
            int heapSize = array.Length;

            buildMaxHeap(array);

            for (int i = heapSize - 1; i >= 1; i--)
            {
                swap(array, i, 0);
                heapSize--;
                sink(array, heapSize, 0);
            }
        }

        private void buildMaxHeap<T>(T[] array) where T : IComparable<T>
        {
            int heapSize = array.Length;

            for (int i = (heapSize / 2) - 1; i >= 0; i--)
            {
                sink(array, heapSize, i);
            }
        }

        private void sink<T>(T[] array, int heapSize, int toSinkPos) where T : IComparable<T>
        {
            if (getLeftKidPos(toSinkPos) >= heapSize)
            {
                // No left kid => no kid at all
                return;
            }


            int largestKidPos;
            bool leftIsLargest;

            if (getRightKidPos(toSinkPos) >= heapSize || array[getRightKidPos(toSinkPos)].CompareTo(array[getLeftKidPos(toSinkPos)]) < 0)
            {
                largestKidPos = getLeftKidPos(toSinkPos);
                leftIsLargest = true;
            }
            else
            {
                largestKidPos = getRightKidPos(toSinkPos);
                leftIsLargest = false;
            }



            if (array[largestKidPos].CompareTo(array[toSinkPos]) > 0)
            {
                swap(array, toSinkPos, largestKidPos);

                if (leftIsLargest)
                {
                    sink(array, heapSize, getLeftKidPos(toSinkPos));

                }
                else
                {
                    sink(array, heapSize, getRightKidPos(toSinkPos));
                }
            }

        }

        private void swap<T>(T[] array, int pos0, int pos1)
        {
            T tmpVal = array[pos0];
            array[pos0] = array[pos1];
            array[pos1] = tmpVal;
        }

        private int getLeftKidPos(int parentPos)
        {
            return (2 * (parentPos + 1)) - 1;
        }

        private int getRightKidPos(int parentPos)
        {
            return 2 * (parentPos + 1);
        }

        private static void printArray<T>(T[] array)
        {

            foreach (T t in array)
            {
                Console.Write(' ' + t.ToString() + ' ');
            }

        }

        //Quick Sort
        private void QuickSort(string[] songs)
        {
            quickSort_REC(songs, 0, songs.Length - 1);
        }

        private void quickSort_REC(string[] songs, int low, int high)
        {
            if (low < high)
            {
                int pi = partition(songs, low, high);
                quickSort_REC(songs, low, pi - 1);
                quickSort_REC(songs, pi + 1, high);
            }
        }

        private int partition(string[] songs, int low, int high)
        {
            string pivot = songs[high];
            int i = (low - 1);

            for (int j = low; j <= high - 1; j++)
            {
                if (songs[j].CompareTo(pivot) < 0)
                {
                    i++;
                    string storeOne = songs[i];
                    songs[i] = songs[j];
                    songs[j] = storeOne;
                }
            }
            string store = songs[i + 1];
            songs[i + 1] = songs[high];
            songs[high] = store;
            return (i + 1);
        }

        //Binary Search
        //static int binarySearch(int[] arr, int l, int r, int x)
        //{
        //   if (r >= l)
        //    {
        //        int mid = l + (r - l) / 2;
        //        if (arr[mid] == x)
        //            return mid;
        //
        //        if (arr[mid] > x)
        //            return binarySearch(arr, l, mid - 1, x);

        //        return binarySearch(arr, mid + 1, r, x);
        //    }
        //    return -1;
        //}

        public int BinarySearch(LinkedList<string> song, String key)
    {
        int total = SongList.Count();
        String[] arr = new string[total];
        arr = SongList.ToArray();
        int minNum = 0;
        int maxNum = arr.Length - 1;

        while (minNum <= maxNum)
        {
            int mid = (minNum + maxNum) / 2;
            int index = arr[mid].LastIndexOf('\\');
            string songName = arr[mid].Substring(index + 1);
            index = songName.LastIndexOf(",");
            songName = songName.Substring(0, index);
            int i = String.Compare(key, songName);

            if (arr[mid].Contains(key))
            {
                MessageBox.Show("Found");
                return mid;
            }
            //else if (key < arr[mid])
            else if (i <= 0)
            {
                maxNum = mid - 1;
            }
            else
            {
                minNum = mid + 1;
            }
        }
        MessageBox.Show("Not Found");
        return -1;
      
        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            int num = BinarySearch(SongList,textBoxSearch.Text);
            listBoxSong.SelectedIndex = num;
            //listBoxSong.Update.
        }
        public void RefreshListBox()
        {
            listBoxSong.Items.Clear();
            foreach (string file in SongList)
            {
                int index = file.LastIndexOf('\\');
                string songName = file.Substring(index + 1);
                index = songName.LastIndexOf(",");
                songName = songName.Substring(0, index);
                listBoxSong.Items.Add(songName);
            }

            string[] sortListBox = listBoxSong.Items.Cast<string>().ToArray();
            QuickSort(sortListBox);
            listBoxSong.Items.Clear();
            foreach (string file in sortListBox)
            {
                listBoxSong.Items.Add(file);
                List<string> arr = new List<string>();

            }
        }

        //Save CSV file
        private void buttonSave_Click(object sender, EventArgs e)
        {   
            using (var sw = new StreamWriter("test.csv"))
            {
                string[] sortLinkedList = SongList.Cast<string>().ToArray();
                QuickSort(sortLinkedList);
                foreach (string songEntry in sortLinkedList)
                {
                    String hashFileName = GetMD5HashFromFile(openFileDialog1.FileName);
                    sw.WriteLine(songEntry);                   
                }
            }
        }

        //Load CSV file
        private void buttonLoad_Click(object sender, EventArgs e)
        {

            SongList.Clear();
            using (StreamReader reader = new StreamReader(new FileStream("test.csv", FileMode.Open)))
            {
                string line;
                // Read line by line  
                while ((line = reader.ReadLine()) != null)
                {
                    SongList.AddLast(line);
                    RefreshListBox();                  
                }
            }
        }
    }
}
