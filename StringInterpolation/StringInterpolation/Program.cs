using System;
using BenchmarkDotNet.Running;
using StringInterpolation;

var summary = BenchmarkRunner.Run<Tests>();

// var date = DateTime.Today;
// ConditionalLog.Log(date.Day == 22, $"Today is {date}");
