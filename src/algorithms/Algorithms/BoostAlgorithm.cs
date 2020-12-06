﻿using SoborniyProject.src.algorithms.CarAndRoads;
using SoborniyProject.src.algorithms.TrafficLights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoborniyProject.src.algorithms.Algorithms
{
    class BoostAlgorithm
    {



        public void BoostWay(List<CarSessions> car_sessions, List<RoadInf> roads, BoostAlgorithm position_Braking_or_Boost, long key)//считает сколько времни надо ехать чтоб преодолеть нужное расстояние
        {
            List<GreenLight> greens = new List<GreenLight>();
            List<RedLight> reds = new List<RedLight>();
            List<YellowLight> yellows = new List<YellowLight>();
            greens.Add(new GreenLight() { });
            reds.Add(new RedLight() { });
            yellows.Add(new YellowLight() { });
            yellows[0].Context = car_sessions[0].Context;
            greens[0].Context = car_sessions[0].Context;
            reds[0].Context = car_sessions[0].Context;
            yellows[0].DB_Inf(yellows, key);
            greens[0].DB_Inf(greens, key);
            reds[0].DB_Inf(reds, key);

            RegroupAndVarification regroup = new RegroupAndVarification();

            regroup.Find_cycle(greens, reds, yellows, regroup);

            for (int iter = 0; iter < roads.Count(); iter++)
            {
                if (iter - 1 > 0 && car_sessions[iter - 1].SpeedBetweenBoostAndBreaking == 0)
                {
                    car_sessions[iter - 1].BreakingTime = 0;
                }
                car_sessions[iter].FullDistance = car_sessions[iter].BoostDistance;//в полный путь занес результат разгона
                if (car_sessions[iter].FullDistance > roads[iter].DistaceRoadSite) { position_Braking_or_Boost.BelowSpeed(car_sessions, roads, iter); iter--; continue; }
                if (car_sessions[iter].DistanceAfterBoost + car_sessions[iter].BoostDistance < roads[iter].DistaceRoadSite - car_sessions[iter].BoostDistance) //если мы достигли нужного расстояния завершаем 
                {
                    car_sessions[iter].TimeAfterBoost = (roads[iter].DistaceRoadSite - car_sessions[iter].BoostDistance) / car_sessions[iter].CurrentSpeed;
                    car_sessions[iter].DistanceAfterBoost = roads[iter].DistaceRoadSite - car_sessions[iter].BoostDistance;
                }
                car_sessions[iter].FullDistance += car_sessions[iter].DistanceAfterBoost;//полный путь
                position_Braking_or_Boost.VerificationOnColour(car_sessions, roads, iter, greens, reds, yellows, regroup);//проверка времени относительно светофора
                if (iter - 1 >= 0)
                {
                    if (car_sessions[iter - 1].SpeedLimit > car_sessions[iter].SpeedLimit)
                    {

                        car_sessions[iter].BoostTime = 0;
                        car_sessions[iter].BoostDistance = 0;
                        car_sessions[iter].DistanceAfterBoost = 0;
                        car_sessions[iter].TimeAfterBoost = 0;
                        car_sessions[iter].CurrentSpeed = car_sessions[iter].SpeedLimit * 1000 / 3600;
                        car_sessions[iter].BreakingTime = ((car_sessions[iter - 1].SpeedLimit - car_sessions[iter].SpeedLimit) * car_sessions[0].BreakingAbility) / 100;
                        car_sessions[iter].BreakinDistance = (100 / car_sessions[0].BreakingAbility * Math.Pow(car_sessions[iter].BreakingTime, 2)) / 2;
                        car_sessions[iter].SpeedBetweenBoostAndBreaking = (car_sessions[iter - 1].SpeedLimit - car_sessions[iter].SpeedLimit) * -1;
                        car_sessions[iter].DistanceAfterBoost = roads[iter].DistaceRoadSite - car_sessions[iter].BreakinDistance;
                        if (car_sessions[iter].DistanceAfterBoost < 0)
                        {
                            position_Braking_or_Boost.GoBack(car_sessions, roads, iter);
                            if (car_sessions[0].SessionLose == 1)
                            {
                                car_sessions[0].SaveSessions(car_sessions, key);
                                break;
                            }
                            if (iter - 2 > 0)
                            {
                                iter = iter - 2;
                            }
                            else
                            {
                                iter = -1;
                            }
                            continue;
                        }
                        car_sessions[iter].TimeAfterBoost = car_sessions[iter].BreakingTime + (roads[iter].DistaceRoadSite / ((car_sessions[iter].SpeedLimit * 1000) / 3600));

                        position_Braking_or_Boost.ZeroingAndCreateNext(car_sessions, roads, iter);
                    }
                    else if (car_sessions[iter - 1].SpeedLimit < car_sessions[iter].SpeedLimit)
                    {
                        car_sessions[iter].BoostDistance = 0;
                        car_sessions[iter].DistanceAfterBoost = 0;
                        car_sessions[iter].TimeAfterBoost = 0;
                        car_sessions[iter].BoostTime = 0;
                        car_sessions[iter].CurrentSpeed = (car_sessions[iter].SpeedLimit * 1000) / (60 * 60);
                        car_sessions[iter].BoostTime = (car_sessions[iter].CurrentSpeed - (car_sessions[iter - 1].SpeedLimit * 1000 / 3600)) / car_sessions[0].AccelerationPerSecond;
                        car_sessions[iter].BoostDistance = (car_sessions[0].AccelerationPerSecond * Math.Pow(car_sessions[iter].BoostTime, 2)) / 2;
                        car_sessions[iter].DistanceAfterBoost = (roads[iter].DistaceRoadSite - car_sessions[iter].BoostDistance);
                        car_sessions[iter].TimeAfterBoost = car_sessions[iter].DistanceAfterBoost / ((car_sessions[iter].SpeedLimit * 1000) / (60 * 60));

                        position_Braking_or_Boost.ZeroingAndCreateNext(car_sessions, roads, iter);
                    }
                    else { position_Braking_or_Boost.ZeroingAndCreateNext(car_sessions, roads, iter); }
                }
                else { position_Braking_or_Boost.ZeroingAndCreateNext(car_sessions, roads, iter); }
            }

        }


        public void ZeroingAndCreateNext(List<CarSessions> car_sessions, List<RoadInf> roads, int iter)
        {
            if (roads.Count > iter + 1)
            {
                car_sessions.Add(new CarSessions());
                car_sessions[iter + 1].DistanceAfterBoost = 0;
                car_sessions[iter + 1].BoostDistance = 0;
                car_sessions[iter + 1].TimeAfterBoost = 0;
                car_sessions[iter + 1].BoostTime = 0;
                if (car_sessions[iter].SpeedLimit < car_sessions[0].CarMaxSpeed)
                {
                    car_sessions[iter + 1].SpeedLimit = car_sessions[0].CarMaxSpeed;
                    car_sessions[iter + 1].CurrentSpeed = car_sessions[iter + 1].SpeedLimit * 1000 / 3600;
                    car_sessions[iter + 1].BoostTime = (car_sessions[iter + 1].CurrentSpeed - (car_sessions[iter].SpeedLimit * 1000 / 3600)) / car_sessions[0].AccelerationPerSecond;
                    car_sessions[iter + 1].BoostDistance = (car_sessions[0].AccelerationPerSecond * Math.Pow(car_sessions[iter + 1].BoostTime, 2)) / 2;
                    car_sessions[iter + 1].DistanceAfterBoost = roads[iter + 1].DistaceRoadSite - car_sessions[iter + 1].BoostDistance;
                    car_sessions[iter + 1].TimeAfterBoost = car_sessions[iter + 1].DistanceAfterBoost / (car_sessions[iter + 1].SpeedLimit * 1000 / 3600);
                }
                else
                {

                    car_sessions[iter + 1].SpeedLimit = car_sessions[iter].SpeedLimit;
                    car_sessions[iter + 1].CurrentSpeed = car_sessions[iter].SpeedLimit * 1000 / 3600;
                    car_sessions[iter + 1].DistanceAfterBoost = roads[iter + 1].DistaceRoadSite;
                    car_sessions[iter + 1].TimeAfterBoost = roads[iter + 1].DistaceRoadSite / (car_sessions[iter].SpeedLimit * 1000 / 3600);
                }
            }
        }

        public void VerificationOnColour(List<CarSessions> car_sessions, List<RoadInf> roads, int iter, List<GreenLight> greens, List<RedLight> reds, List<YellowLight> yellows, RegroupAndVarification regroup)
        {
            regroup.Regroup(greens, reds, yellows, regroup, car_sessions, roads, iter);
        }

        public void BelowSpeed(List<CarSessions> car_sessions, List<RoadInf> roads, int iter)
        {

            while (roads[iter].DistaceRoadSite < car_sessions[iter].BoostDistance)
            {
                car_sessions[iter].SpeedLimit = car_sessions[iter].SpeedLimit - 5;
                car_sessions[iter].CurrentSpeed = car_sessions[iter].SpeedLimit * 1000 / 3600;
                car_sessions[iter].BoostTime = (car_sessions[iter].CurrentSpeed) / car_sessions[0].AccelerationPerSecond;
                car_sessions[iter].BoostDistance = (car_sessions[0].AccelerationPerSecond * Math.Pow(car_sessions[iter].BoostTime, 2)) / 2;
                car_sessions[iter].DistanceAfterBoost = 0;
                car_sessions[iter].TimeAfterBoost = car_sessions[iter].DistanceAfterBoost / (car_sessions[iter].SpeedLimit * 1000 / 3600);
                car_sessions[iter].FullDistance = 0;
                if (car_sessions[iter].SpeedLimit < 5) { car_sessions[0].SessionLose = 1; }

            }
        }

        public void GoBack(List<CarSessions> car_sessions, List<RoadInf> roads, int iter)
        {
            double speed = car_sessions[iter - 1].SpeedLimit;
            while (car_sessions[iter].DistanceAfterBoost < 0)
            {
                if ((car_sessions[iter - 1].SpeedLimit - 5) * 1000 / 3600 < car_sessions[iter].SpeedLimit) { break; }
                speed = speed - 5;
                car_sessions[iter].BoostTime = 0;
                car_sessions[iter].BoostDistance = 0;
                car_sessions[iter].DistanceAfterBoost = 0;
                car_sessions[iter].TimeAfterBoost = 0;
                car_sessions[iter].CurrentSpeed = car_sessions[iter].SpeedLimit * 1000 / 3600;
                car_sessions[iter].BreakingTime = ((speed - car_sessions[iter].SpeedLimit) * car_sessions[0].BreakingAbility) / 100;
                car_sessions[iter].BreakinDistance = (100 / car_sessions[0].BreakingAbility * Math.Pow(car_sessions[iter].BreakingTime, 2)) / 2;
                car_sessions[iter].SpeedBetweenBoostAndBreaking = (car_sessions[iter - 1].SpeedLimit - car_sessions[iter].SpeedLimit) * -1;
                car_sessions[iter].DistanceAfterBoost = roads[iter].DistaceRoadSite - car_sessions[iter].BreakinDistance;
            }
            car_sessions[iter - 1].SpeedLimit = speed;
            car_sessions.RemoveAt(iter);
            if (iter - 2 > 0)
            {
                if (car_sessions[iter - 1].SpeedLimit > car_sessions[iter - 2].SpeedLimit)
                {
                    car_sessions[iter - 1].CurrentSpeed = car_sessions[iter - 1].SpeedLimit * 1000 / 3600;
                    car_sessions[iter - 1].BoostTime = (car_sessions[iter - 1].CurrentSpeed) / car_sessions[0].AccelerationPerSecond;
                    car_sessions[iter - 1].BoostDistance = (car_sessions[0].AccelerationPerSecond * Math.Pow(car_sessions[iter - 1].BoostTime, 2)) / 2;
                    car_sessions[iter - 1].DistanceAfterBoost = 0;
                    car_sessions[iter - 1].TimeAfterBoost = car_sessions[iter - 1].DistanceAfterBoost / (car_sessions[iter - 1].SpeedLimit * 1000 / 3600);
                    car_sessions[iter - 1].FullDistance = 0;
                }
                else
                {
                    car_sessions[iter - 1].BoostTime = 0;
                    car_sessions[iter - 1].BoostDistance = 0;
                    car_sessions[iter - 1].DistanceAfterBoost = 0;
                    car_sessions[iter - 1].TimeAfterBoost = 0;
                    car_sessions[iter - 1].CurrentSpeed = car_sessions[iter - 1].SpeedLimit * 1000 / 3600;
                    car_sessions[iter - 1].BreakingTime = ((car_sessions[iter - 2].SpeedLimit - car_sessions[iter - 1].SpeedLimit) * car_sessions[0].BreakingAbility) / 100;
                    car_sessions[iter - 1].BreakinDistance = (100 / car_sessions[0].BreakingAbility * Math.Pow(car_sessions[iter - 1].BreakingTime, 2)) / 2;
                    car_sessions[iter - 1].SpeedBetweenBoostAndBreaking = (car_sessions[iter - 1].SpeedLimit - car_sessions[iter - 1].SpeedLimit) * -1;
                    car_sessions[iter - 1].DistanceAfterBoost = roads[iter - 1].DistaceRoadSite - car_sessions[iter - 1].BreakinDistance;
                }
            }
            else
            {
                car_sessions[iter - 1].CurrentSpeed = car_sessions[iter - 1].SpeedLimit * 1000 / 3600;
                car_sessions[iter - 1].BoostTime = (car_sessions[iter - 1].CurrentSpeed) / car_sessions[0].AccelerationPerSecond;
                car_sessions[iter - 1].BoostDistance = (car_sessions[0].AccelerationPerSecond * Math.Pow(car_sessions[iter - 1].BoostTime, 2)) / 2;
                car_sessions[iter - 1].DistanceAfterBoost = 0;
                car_sessions[iter - 1].TimeAfterBoost = car_sessions[iter - 1].DistanceAfterBoost / (car_sessions[iter - 1].SpeedLimit * 1000 / 3600);
                car_sessions[iter - 1].FullDistance = 0;
            }

        }

        public void ClearCarSessionsData() 
        {

        }

    }
}

