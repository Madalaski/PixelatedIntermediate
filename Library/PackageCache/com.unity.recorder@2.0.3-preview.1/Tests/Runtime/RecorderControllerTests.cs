﻿using System;
using NUnit.Framework;
using UnityEditor.Recorder;

namespace UnityEngine.Recorder.Tests
{
	class RecorderControllerTests
	{
		[Test]
		public void StartRecording_WithNullSettings_ThrowsException()
		{
			var recorderController = new RecorderController(null);

			var ex = Assert.Throws<NullReferenceException>(() => recorderController.StartRecording());
			Assert.IsTrue(ex.Message.Contains("Can start recording without prefs"));
			Assert.IsFalse(recorderController.IsRecording());
		}
		
		[Test]
		public void StartRecording_WithEmptySettings_ShouldNotStartRecording()
		{
			var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
			var recorderController = new RecorderController(settings);

			Assert.IsFalse(recorderController.StartRecording());
			Assert.IsFalse(recorderController.IsRecording());
			
			Object.DestroyImmediate(settings);
		}
		
		[Test]
		public void StartAndStopRecording_WithValidSettings_ShouldStartThenStopRecording()
		{
			var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
			var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
			
			settings.AddRecorderSettings(imageRecorder);
			var recorderController = new RecorderController(settings);

			Assert.IsTrue(recorderController.StartRecording());
			Assert.IsTrue(recorderController.IsRecording());
			
			recorderController.StopRecording();
			Assert.IsFalse(recorderController.IsRecording());
			
			Object.DestroyImmediate(imageRecorder);
			Object.DestroyImmediate(settings);
		}
	}
}
