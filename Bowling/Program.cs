﻿using System;
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
                var dataPoints = response.Data.Points;

                //List<string> dataPoints = new List<string>();
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");
                //dataPoints.Add("[10,0]");


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
                //check if this frame is neihter a strike or a spare
                if (ballThrowList[frame].FirstThrow + ballThrowList[frame].SecondThrow < 10)
                {
                    sum += ballThrowList[frame].FirstThrow +
                           ballThrowList[frame].SecondThrow;
                }
                //check if this frame is a spare
                else if (ballThrowList[frame].FirstThrow + ballThrowList[frame].SecondThrow == 10 &&
                    ballThrowList[frame].FirstThrow != 10)
                {


                    currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow;
                    sum += currentFrameScore;
                }
                //check if this frame is a strike
                else if (ballThrowList[frame].FirstThrow == 10 && ballThrowList[frame].SecondThrow == 0)
                {
                    //check if this is not the first throw of the game
                    if (frame > 0 && ballThrowList[frame - 1].FirstThrow == 10)
                    {
                        //check that this is the last frame with 3 throws
                        if (frame == 10)
                        {

                            if (ballThrowList.Count >= frame + 2 && frame < 10)
                            {
                                if (ballThrowList[frame + 1].FirstThrow == 10 && ballThrowList[frame + 2].FirstThrow == 10)
                                {

                                    currentFrameScore = 30;
                                    sum += currentFrameScore;
                                }

                                else if (ballThrowList[frame + 1].FirstThrow == 10 && ballThrowList[frame + 2].FirstThrow < 10)
                                {

                                    currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                                    sum += currentFrameScore;
                                }

                                else
                                {

                                    currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                                    sum += currentFrameScore;
                                }
                            }
                            //check for triple strike streak has stopped and has to check the next 2 throw, which first next is a strike and the second next is not
                            else if (ballThrowList.Count >= frame + 2 && frame < 10)
                            {
                                if (ballThrowList[frame + 1].FirstThrow == 10 && ballThrowList[frame + 2].FirstThrow < 10)
                                {

                                    currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                                    sum += currentFrameScore;
                                }


                                else
                                {

                                    currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                                    sum += currentFrameScore;
                                }
                            }
                        }
                        //if not all frames are played and triple strike are made
                        else if (ballThrowList.Count >= frame + 2 && ballThrowList.Count < 11)
                        {
                            //if triple strike are made
                            if (ballThrowList[frame + 1].FirstThrow == 10 && ballThrowList[frame + 2].FirstThrow == 10)
                            {

                                currentFrameScore = 30;
                                sum += currentFrameScore;
                            }
                            //if triple strike streak is stopped but first next throw is still a strike
                            else if (ballThrowList[frame + 1].FirstThrow == 10 && ballThrowList[frame + 2].FirstThrow < 10)
                            {

                                currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                                sum += currentFrameScore;
                            }

                            //if triple strike streak stopped and next first throw is not a strike
                            else
                            {

                                currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                                sum += currentFrameScore;
                            }
                        }
                        else
                        {
                            currentFrameScore = 30;
                            sum += currentFrameScore;
                        }


                    }

                    else
                    {

                        if (ballThrowList[frame + 1].FirstThrow == 10)
                        {

                            currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                            sum += currentFrameScore;
                        }
                        else
                        {

                            currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                            sum += currentFrameScore;
                        }

                    }

                }




                sumArray.Add(sum);
            }

            string postScore = "";


            if (ballThrowList.Count == 11)
            {
                sum += ballThrowList[10].FirstThrow + ballThrowList[10].SecondThrow;
                sumArray.Add(sum);
                sumArray.RemoveAt(10);

            }
            else
            {
                sum += ballThrowList.Last().FirstThrow + ballThrowList.Last().SecondThrow;
                sumArray.Add(sum);

            }


            int[] myints = sumArray.ToArray();

            postScore = String.Join(",", myints.Select(p => p.ToString()).ToArray());

            return postScore;
        }
    }
}
