using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishClassifier
{

	public class KMeans
	{
		int[] clusterIDs;
		double[,] means;
		int[] clusterSizes;

		private void InitClusters(int[,] data, int k)
		{
			int m = data.GetLength(0);
			int n = data.GetLength(1);
			clusterIDs = new int[m];
			means = new double[k, n];
			clusterSizes = new int[k];

			for (int i = 0; i < m; i++)
			{
				clusterIDs[i] = i % k;
			}
		}
		//Input: Takes int array
		//Output: return double array
		//Logic: calculate gaussian distance from each means which kmeans has calculated 
		private double[] CalculateDistanceFromMean(int[] sampleData)
        {
			// Sqrt[(x2-x1)^2 + (y2-y1)^2] 

			int[,] squaredValue = new int[2, 3];
			
			double[] rootSquaredDistance = new double[2];
			for (int i = 0; i < means.GetLength(0); i++)
			{
				double distance = 0;
				for (int j = 0; j < means.GetLength(1); j++)
				{
					squaredValue[i, j] = Convert.ToInt32(Math.Pow(Convert.ToInt32(means[i, j]) - sampleData[j], 2));
					distance = distance + squaredValue[i, j];
				}

				rootSquaredDistance[i] = Math.Sqrt(distance);
			}

			return rootSquaredDistance;
		}


		//Input: Takes int array
		//Output: return keyvaluepair
		//Logic: returns the value which has minimum distance from mean
		public KeyValuePair<int,double> PredictClass(int[] sampleData)
        {
			double[] distance = CalculateDistanceFromMean(sampleData);
			KeyValuePair<int, double> result = new KeyValuePair<int, double>();
			if (distance[0] > distance[1])
            {
				result = new KeyValuePair<int, double>(1, distance[1]);
            }
            else
            {
				result = new KeyValuePair<int, double>(0, distance[0]);
			}
			return result;

		}
		private void UpdateMeans(int[,] data)
		{
			int m = data.GetLength(0);
			int n = data.GetLength(1);
			int k = clusterSizes.Length;
			for (int i = 0; i < k; i++)
			{
				for (int j = 0; j < n; j++)
				{
					means[i, j] = 0.0;
				}
				clusterSizes[i] = 0;
			}
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					means[clusterIDs[i], j] += data[i, j];
				}
				clusterSizes[clusterIDs[i]]++;
			}
			for (int i = 0; i < k; i++)
			{
				for (int j = 0; j < n; j++)
				{
					means[i, j] /= clusterSizes[i];
				}
			}
		}

		private void UpdateClusterIDs(int[,] data)
		{
			int m = data.GetLength(0);
			int n = data.GetLength(1);
			int k = clusterSizes.Length;
			for (int i = 0; i < m; i++)
			{
				double smallestDistanceSqr = 0.0;
				int bestClusterID = clusterIDs[i];
				for (int j = 0; j < n; j++)
				{
					double diffJ = data[i, j] - means[clusterIDs[i], j];
					smallestDistanceSqr += diffJ * diffJ;
				}

				for (int clusterID = 0; clusterID < k; clusterID++)
				{
					if (clusterID == clusterIDs[i])
						continue;
					double distanceSqr = 0.0;
					for (int j = 0; j < n; j++)
					{
						double diffJ = data[i, j] - means[clusterID, j];
						distanceSqr += diffJ * diffJ;
					}
					if (distanceSqr < smallestDistanceSqr)
					{
						smallestDistanceSqr = distanceSqr;
						clusterIDs[i] = clusterID;
					}
				}
			}
		}
		public KMeans(int[,] data, int k, int numRounds)
		{
			InitClusters(data, k);

			for (int round = 0; round < numRounds; round++)
			{
				UpdateMeans(data);
				UpdateClusterIDs(data);
			}
		}

		public int[] GetClusterIDs()
		{
			return clusterIDs;
		}
	}

}
