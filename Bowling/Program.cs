using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using RestSharp;

namespace Bowling
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var client = new RestClient("http://13.74.31.101/api");
                var request = new RestRequest("/points", Method.GET);
                request.AddHeader("Content-Type", "application/json");

                IRestResponse<Score> response = client.Execute<Score>(request);
                //var dataPoints = response.Data.Points;

                List<string> dataPoints = new List<string>();

                dataPoints.Add("[10,0]");
                dataPoints.Add("[10,0]");
                dataPoints.Add("[7,2]");
                dataPoints.Add("[7,3]");
                dataPoints.Add("[10,0]");
                dataPoints.Add("[10,0]");
                dataPoints.Add("[1,0]");


                var token = response.Data.Token;

                List<FrameScore> ballThrowList = new List<FrameScore>();
                List<int[]> frames = new List<int[]>();

                TrimData(dataPoints, frames, ballThrowList);


                var postScore = PostScore(ballThrowList, frames, dataPoints);


                var postRequest = new RestRequest("/points", Method.POST);
                postRequest.AddParameter("token", token);
                postRequest.AddParameter("points", "[" + postScore + "]");
                IRestResponse response2 = client.Execute(postRequest);
                var postContent = response2.Content;
                Console.WriteLine(postContent);




                Console.ReadKey();
            }
        }
        //Trim the data from json to list
        private static void TrimData(List<string> dataPoints, List<int[]> frames, List<FrameScore> ballThrowList)
        {
            foreach (var var in dataPoints)
            {
                string trimmed = var.TrimStart('[').TrimEnd(']');
                string[] split = trimmed.Split(',');
                int[] myInts = Array.ConvertAll(split, s => int.Parse(s));
                frames.Add(myInts);
            }

            foreach (var var in frames)
            {
                FrameScore thrown = new FrameScore();
                thrown.FirstThrow = var[0];
                thrown.SecondThrow = var[1];
                ballThrowList.Add(thrown);
            }
        }

        private static string PostScore(List<FrameScore> ballThrowList, List<int[]> frames, List<string> dataPoints)
        {
            int sum = 0;

            List<int> sumArray = new List<int>();
            for (int frame = 0; frame < ballThrowList.Count - 1; frame++)
            {
                Console.WriteLine(ballThrowList[frame].FirstThrow + " " + ballThrowList[frame].SecondThrow);
                int currentFrameScore = 0;

                if (ballThrowList[frame].FirstThrow + ballThrowList[frame].SecondThrow < 10)
                {
                    sum += ballThrowList[frame].FirstThrow +
                           ballThrowList[frame].SecondThrow;
                }
                //this check if it is a Spare
                else if (ballThrowList[frame].FirstThrow + ballThrowList[frame].SecondThrow == 10 &&
                    ballThrowList[frame].FirstThrow != 10)
                {
                    Console.WriteLine("Spare");

                    currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow;
                    sum += currentFrameScore;
                }
                //this check if it is  a Strike
                else if (ballThrowList[frame].FirstThrow == 10 && ballThrowList[frame].SecondThrow == 0)
                {
                    //check if this is not the first throw at the game, to check if previous thorw was a strike
                    if (frame > 0 && ballThrowList[frame - 1].FirstThrow == 10)
                    {

                        //check to see if next throw is also a strike
                        if (ballThrowList[frame + 1].FirstThrow == 10)
                        {
                            Console.WriteLine("Turkey");
                            currentFrameScore = 30;
                            sum += currentFrameScore;
                        }
                        //if next throw is not a strike, add points now plus the next frames score
                        else
                        {
                            Console.WriteLine("Double Strike");
                            currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                            sum += currentFrameScore;
                        }
                    }
                    //if this is the first throw with a strike
                    else
                    {
                        //if next throw also is a strike, add 20 and also the first throw from the next 2 frames from now
                        if (ballThrowList[frame + 1].FirstThrow == 10)
                        {
                            Console.WriteLine("Double Strike");
                            currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                            sum += currentFrameScore;
                        }
                        else
                        {
                            //if next throw is not a strike, add 10 points plus next frame throws
                            Console.WriteLine("Strike");
                            currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                            sum += currentFrameScore;
                        }

                    }

                }




                sumArray.Add(sum);
            }

            string postScore = "";


            if (frames.Count == 11)
            {
                sum += frames[10][0] + frames[10][1];
                sumArray.Add(sum);
                sumArray.RemoveAt(10);

            }
            else
            {
                sum += frames.Last().Sum();
                sumArray.Add(sum);

            }


            int[] myints = sumArray.ToArray();

            postScore = String.Join(",", myints.Select(p => p.ToString()).ToArray());

            return postScore;
        }
    }
}
