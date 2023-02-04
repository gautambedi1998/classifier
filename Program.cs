using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FishClassifier
{
    class Program
    {

        //Main dataset
        private static List<string> dataSet = new List<string>();
        private const int dataColumns = 3;

        //Input: Takes string collection
        //Output: return int matrix in r,g,b
        //Logic: converts string r,g,b to int r,g,b value
        private static int[,] ConvertStringToIntegerArray(List<string> fields)
        {
            int[,] data = new int[fields.Count, dataColumns];
            for (int i = 0; i < fields.Count; i++)
            {
                string[] field = fields[i].Split(',');
                for (int j = 0; j < dataColumns; j++)
                {
                    data[i, j] = Convert.ToInt32(field[j]);
                }
            }

            return data;
        }


        //Input: Takes string value
        //Output: return int array in r,g,b
        //Logic: converts string r,g,b to int r,g,b value
        private static int[] ConvertStringToIntegerArray(string fields)
        {
            //int[,] data = new int[1, dataColumns];
            //for (int i = 0; i < 1; i++)
            //{
            //    string[] field = fields.Split(',');
            //    for (int j = 0; j < dataColumns; j++)
            //    {
            //        data[i, j] = Convert.ToInt32(field[j]);
            //    }
            //}
            int[] data = new int[3]; 
            string[] field = fields.Split(',');
            for (int j = 0; j < dataColumns; j++)
            {
                data[j] = Convert.ToInt32(field[j]);
            }

            return data;
        }

        //Input: Takes only file Name
        //Output: array of integers, filled with csv file data.
        //Logic: extract data from csv and slice into r,g,b only, and converted into int array
        private static int[,] loadCSV(string fileName)
        {
         
            string[] linesInCSV = File.ReadAllLines(fileName);
            int numSamples = linesInCSV.Length;

            const int numFields = 3; // The three fields are  r, g, b
            int[,] data = new int[numSamples, numFields];
            for (int i = 0; i < numSamples; i++)
            {
                string[] fields = linesInCSV[i].Split(',');
                for (int j = 0; j < numFields; j++)
                {
                    data[i, j] = Convert.ToInt32(fields[j+2]);
                }
            }
            return data;
        }
        //Input: Takes int array
        //Output: one string r,g,b value
        //Logic: compute average of r ,g and b of each int array. seprated by ,
        private static string ComputeAvgRGBFromEachFile(int[,] data)
        {
            int[] r = new int[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {     
                  r[i] = data[i, 0];           
            }

            double rAvg = r.Average();

            int[] g = new int[data.GetLength(0)];
            for (int i = 1; i < data.GetLength(0); i++)
            {
                g[i] = data[i,1];
            }

            double gAvg = g.Average();

            int[] b = new int[data.GetLength(0)];
            for (int i = 2; i < data.GetLength(0); i++)
            {
                b[i] = data[i,2];    
            }

            double bAvg = b.Average();

            //Console.WriteLine("R avg = " + rAvg);
            //Console.WriteLine("G avg = " + gAvg);
            //Console.WriteLine("B avg = " + bAvg);
            //Console.WriteLine("===================");

            string dataPoint = Convert.ToInt32(rAvg) + "," + Convert.ToInt32(gAvg) + "," + Convert.ToInt32(bAvg);
            return dataPoint;
            
        }


        //Input: Folder Paths of fishes and non-fishes
        //Output: Dataset Populated
        //Logic: Gets Files from directory and passing a helper function called ExtractAndPrepareDataFromFiles()
        private static void InitilizeDataSet()
        {

            string folderPathForFish = "../../DataSet/Training/Fish";
            string folderPathForNoFish = "../../DataSet/Training/NoFish";

            DirectoryInfo d = new DirectoryInfo(folderPathForFish); 

            FileInfo[] fishFiles = d.GetFiles("*.csv"); //Getting csv files

            DirectoryInfo d2 = new DirectoryInfo(folderPathForNoFish);

            FileInfo[] noFishFiles = d2.GetFiles("*.csv"); //Getting csv files


            ExtractAndPrepareDataFromFiles(fishFiles);
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("No fish starts from here");
            ExtractAndPrepareDataFromFiles(noFishFiles);

           
          

         }


        private static void InitilizeDataSet(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);

            FileInfo[] allFiles = d.GetFiles("*.csv"); //Getting csv files

            ExtractAndPrepareDataFromFiles(allFiles);

        }


        //Input: File Paths of fishes and nonfishes
        //Output: Populated dataSet property
        //Logic: Gets Files  and passing a helper function called loadCSV(), then the result will be converted and add the dataSet collection.
        private static void ExtractAndPrepareDataFromFiles(FileInfo[] files)
        {
            foreach (var file in files)
            {
                var data = loadCSV(file.FullName);
                dataSet.Add(ComputeAvgRGBFromEachFile(data)); 
            }

        }


        //Input: int array and k value, which is centroid of dataset
        //Output: write the output
        //Logic: when data is populated, kmeans algo takes data and process its centroid, after that we predict (PredictClass()) by giving sample data using GetPredictionDataFromFolder() function
        private static void ApplyKMeans(int[,] data, int k)
        {
            
            KMeans kmeans = new KMeans(data, k, 30);
            
            // [12,3,4]
            var result = kmeans.PredictClass(GetPredictionDataFromFolder());

            if(result.Key == 0)
            {
                Console.WriteLine("Fish Detected");
                           
            }
            else
            {
                Console.WriteLine("No Fish Detected");
            }
        }


        private static bool  ApplyKMeans(int[,] data, int[] predictData , int k)
        {

            KMeans kmeans = new KMeans(data, k, 30);

            // [12,3,4]
            var result = kmeans.PredictClass(predictData);

            if (result.Key == 0)
            {
               // Console.WriteLine("Fish Detected");
                return true; // fish is detected

            }
            //else
            //{

            //    Console.WriteLine("No Fish Detected");
            //}
           // Console.WriteLine("No Fish Detected");
            return false; // no fish is detected
        }




        //Input: full path of sample data
        //Output: int array of average sample data
        //Logic: Gets File from directory and passing a helper function called ComputeAvgRGBFromEachFile() which will be converted to int array using ConvertStringToIntegerArray()
        private static int[] GetPredictionDataFromFolder()
        {
            string testData = "../../DataSet/Testing/nofish.csv";

            string data = ComputeAvgRGBFromEachFile(loadCSV(testData));

            return ConvertStringToIntegerArray(data);
        }



        public static byte[] LoadBMP(string filename)
        {
            BitmapImage bmp1 = new BitmapImage(new Uri(filename, UriKind.Relative));
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp1));
            MemoryStream bmp1Stream = new MemoryStream();
            enc.Save(bmp1Stream);
            byte[] bmp1Data = bmp1Stream.ToArray();
            return bmp1Data;
        }


        private static void ConvertImageIntoCSV(string filePath)
        {
            byte[] frameData = LoadBMP(filePath);
            List<string> lines = new List<string>();
            int height = 50, width = 50;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int k = ((height - 1 - y) * width + x) * 4 + 54;
                    byte blue = frameData[k];//Blue;
                    byte green = frameData[k + 1];//Green
                    byte red = frameData[k + 2];//Red
                    lines.Add(string.Format("{0},{1},{2},{3},{4}", x, y, red, green, blue));
                }
            }
            string dir = System.IO.Path.GetDirectoryName(filePath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string csvFileName = System.IO.Path.Combine(dir, fileName + "_selection.csv");
            File.WriteAllLines(csvFileName, lines);

        }


        public static void ConvertAllImageSegmentsIntoCSV(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);

            FileInfo[] fishFiles = d.GetFiles("*.bmp"); //Getting bmp files

            foreach (var item in fishFiles)
            {
                ConvertImageIntoCSV(item.FullName);
            }

        }

        public static void SegmentImageIntoBlocks(string filePath, string outputfolderPath)
        {
            Bitmap image = new Bitmap(filePath);

            // Get the dimensions of the image
            int width = image.Width;
            int height = image.Height;

            // Segment the image into 50x50 pixel blocks
            for (int row = 0; row < height; row += 50)
            {
                for (int col = 0; col < width; col += 50)
                {
                    // Create a new Bitmap object for the current block
                    Bitmap block = new Bitmap(50, 50);

                    // Set the pixels of the block
                    for (int y = 0; y < 50; y++)
                    {
                        for (int x = 0; x < 50; x++)
                        {
                            if (col + x < width && row + y < height)
                            {
                                Color pixelColor = image.GetPixel(col + x, row + y);
                                block.SetPixel(x, y, pixelColor);
                            }
                        }
                    }

                    // Save the block to a file
                    string blockFilePath = Path.Combine(outputfolderPath, $"Block_{row}_{col}.bmp");

                    block.Save(blockFilePath);
                }
            }
        }

        public static void ApplyKMeansForEachSegment(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);

            FileInfo[] fishFiles = d.GetFiles("*.csv"); //Getting csv files

            foreach (var item in fishFiles)
            {
                string data = ComputeAvgRGBFromEachFile(loadCSV(item.FullName));
                var intData = ConvertStringToIntegerArray(data);
               if(ApplyKMeans(ConvertStringToIntegerArray(dataSet), intData, 2))
                {                   
                    Console.WriteLine("Fish is Detected");
                    Console.WriteLine("Path = " + item.FullName);
                    return;
                }
            }

            Console.WriteLine("Fish is not detected on a frame");
        }

        public static void BuildAndPredictFish(string folderPath)
        {
            
            
            ApplyKMeansForEachSegment(folderPath);

        }
        [STAThread]
        static void Main(string[] args)
        {
            //Step1 : Populate Dataset
           // InitilizeDataSet();

            //if (dataSet == null && dataSet.Count <= 0)
            //{
            //    Console.WriteLine("No training dataset found.");
            //    return;
            //}

            //Step 2: Learn and Predict fish
            //ApplyKMeans(ConvertStringToIntegerArray(dataSet), 2);




            /// For Segments
            /// Step 1
            /// Segment Image
            /// 
            string folderPath = @"../../DataSet/Segments";
            string fileName = string.Empty;
            using (var opnDlg = new OpenFileDialog()) 
            {
                opnDlg.Filter = "Bmp Files (*.bmp)|*.bmp";
                Console.WriteLine("Please Select a BMP image to be segmented");
                if (opnDlg.ShowDialog() == DialogResult.OK)
                {
                    fileName = opnDlg.FileName;
                   
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("There is an issue while uploading the image.");
                Console.ReadKey();
                return;
            }
            /// Step 2
            /// Convert Frame to blocks
            SegmentImageIntoBlocks(fileName, folderPath);

            /// Step 3
            /// Convert images into cvs
            ConvertAllImageSegmentsIntoCSV(folderPath);

            /// Step 4
            /// Populate Dataset
            /// 
            InitilizeDataSet();

            if (dataSet == null && dataSet.Count <= 0)
            {
                Console.WriteLine("No training dataset found.");
                 Console.ReadKey();
                return;
            }

            //Step 5: Learn and Predict fish
            BuildAndPredictFish(folderPath);

            Console.ReadKey();
        }
    }
}
