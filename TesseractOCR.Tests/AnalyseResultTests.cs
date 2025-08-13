﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Tesseract.Tests
{
    [TestClass]
    public class AnalyseResultTests : TesseractTestBase
    {
        #region Fields
        private static string ResultsDirectory => TestResultPath("Analysis/");
        private const string ExampleImagePath = "Ocr/phototest.tif";
        #endregion

        #region Setup\TearDown
        private Engine _engine;

        [TestCleanup]
        public void Dispose()
        {
            _engine?.Dispose();
            _engine = null;
        }

        [TestInitialize]
        public void Init()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);
            _engine = CreateEngine(Language.Osd);
        }
        #endregion Setup\TearDown

        #region Tests
        [DataTestMethod]
        [DataRow(null)]
        [DataRow(90f)]
        [DataRow(180f)]
        public void AnalyseLayout_RotatedImage(float? angle)
        {
            using var img = LoadTestImage(ExampleImagePath);
            using var rotatedImage = angle.HasValue ? img.Rotate(TesseractOCR.Helpers.Math.ToRadians(angle.Value)) : img.Clone();
            rotatedImage.Save(TestResultRunFile($"AnalyseResult/AnalyseLayout_RotateImage_{angle}.png"));

            _engine.DefaultPageSegMode = PageSegMode.AutoOsd;
            using var page = _engine.Process(rotatedImage);
            using var layout = page.Layout;
            foreach(var block in layout)
            {
                var properties = block.Properties;

                ExpectedOrientation(angle ?? 0, out var orient);
                Assert.AreEqual(properties.Orientation, orient);

                if (angle.HasValue)
                {
                    switch (angle)
                    {
                        case 180f:
                            // This isn't correct...
                            Assert.AreEqual(properties.WritingDirection, WritingDirection.LeftToRight);
                            Assert.AreEqual(properties.TextLineOrder, TextLineOrder.TopToBottom);
                            break;

                        case 90f:
                            Assert.AreEqual(properties.WritingDirection, WritingDirection.LeftToRight);
                            Assert.AreEqual(properties.TextLineOrder, TextLineOrder.TopToBottom);
                            break;

                        default:
                            Assert.Fail("Angle not supported.");
                            break;
                    }
                }
                else
                {
                    Assert.AreEqual(properties.WritingDirection, WritingDirection.LeftToRight);
                    Assert.AreEqual(properties.TextLineOrder, TextLineOrder.TopToBottom);
                }
            }
        }

        //[TestMethod]
        //public void CanDetectOrientationForMode(
        //	//[Values(PageSegMode.Auto,
        //	//    PageSegMode.AutoOnly,
        //	//    PageSegMode.AutoOsd,
        //	//    PageSegMode.CircleWord,
        //	//    PageSegMode.OsdOnly,
        //	//    PageSegMode.SingleBlock,
        //	//    PageSegMode.SingleBlockVertText,
        //	//    PageSegMode.SingleChar,
        //	//    PageSegMode.SingleColumn,
        //	//    PageSegMode.SingleLine,
        //	//    PageSegMode.SingleWord)]
        //	//PageSegMode pageSegMode
        //	)
        //{
        //	foreach (var pageSegMode in new[] {
        //	PageSegMode.Auto,
        //		PageSegMode.AutoOnly,
        //		PageSegMode.AutoOsd,
        //		PageSegMode.CircleWord,
        //		PageSegMode.OsdOnly,
        //		PageSegMode.SingleBlock,
        //		PageSegMode.SingleBlockVertText,
        //		PageSegMode.SingleChar,
        //		PageSegMode.SingleColumn,
        //		PageSegMode.SingleLine,
        //		PageSegMode.SingleWord
        //	})
        //		using (var img = LoadTestImage(ExampleImagePath))
        //		{
        //			using (var rotatedPix = img.Rotate((float)Math.PI))
        //			{
        //				using (var page = engine.Process(rotatedPix,pageSegMode))
        //				{
        //					int orientation;
        //					float confidence;
        //					string scriptName;
        //					float scriptConfidence;

        //					page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

        //					Assert.AreEqual(orientation,180);
        //					Assert.AreEqual(scriptName,"Latin");
        //				}
        //			}
        //		}
        //}

        //[DataTestMethod]
        //[DataRow(0)]
        //[DataRow(90)]
        //[DataRow(180)]
        //[DataRow(270)]
        //public void DetectOrientation_Degrees_RotatedImage(int expectedOrientation)
        //{
        //	using (var img = LoadTestImage(ExampleImagePath))
        //	{
        //		using (var rotatedPix = img.Rotate((float)expectedOrientation / 360 * (float)Math.PI * 2))
        //		{
        //			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
        //			{

        //				int orientation;
        //				float confidence;
        //				string scriptName;
        //				float scriptConfidence;

        //				page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

        //				Assert.AreEqual(orientation,expectedOrientation);
        //				Assert.AreEqual(scriptName,"Latin");
        //			}
        //		}
        //	}
        //}

        //[DataTestMethod]
        //[DataRow(0)]
        //[DataRow(90)]
        //[DataRow(180)]
        //[DataRow(270)]
        //public void DetectOrientation_Legacy_RotatedImage(int expectedOrientationDegrees)
        //{
        //	using (var img = LoadTestImage(ExampleImagePath))
        //	{
        //		using (var rotatedPix = img.Rotate((float)expectedOrientationDegrees / 360 * (float)Math.PI * 2))
        //		{
        //			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
        //			{
        //				Orientation orientation;
        //				float confidence;

        //				page.DetectBestOrientation(out orientation,out confidence);

        //				Orientation expectedOrientation;
        //				float expectedDeskew;
        //				ExpectedOrientation(expectedOrientationDegrees,out expectedOrientation,out expectedDeskew);

        //				Assert.AreEqual(orientation,expectedOrientation);
        //			}
        //		}
        //	}
        //}


        [TestMethod]
        public void GetImage(
            //[Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)] PageIteratorLevel level,
            //[Values(0, 3)] int padding
        )
        {
            // TODO : Fix this
            //foreach (var level in new[]
            //         {
            //             PageIteratorLevel.Block,
            //             PageIteratorLevel.Paragraph,
            //             PageIteratorLevel.TextLine,
            //             PageIteratorLevel.Word,
            //             PageIteratorLevel.Symbol
            //         }
            //        )
            
            //using (var img = LoadTestImage(ExampleImagePath))
            //{
            //    using (var page = _engine.Process(img))
            //    {
            //        using (var layout = page.Layout)
            //        {
            //            using (var elementImg = pageLayout.Image)
            //            {
            //                var elementImgFilename = $@"AnalyseResult\GetImage\ResultIterator_Image_{level}.png";

            //                var destFilename = TestResultRunFile(elementImgFilename);
            //                elementImg.Save(destFilename, ImageFormat.Png);
            //            }
            //        }
            //    }
            //}
        }
        #endregion Tests

        #region Helpers
        private static void ExpectedOrientation(float rotation, out Orientation orientation)
        {
            rotation %= 360f;
            rotation = rotation < 0 ? rotation + 360 : rotation;

            switch (rotation)
            {
                case >= 315:
                case < 45:
                    orientation = Orientation.PageUp;
                    break;
                case >= 45 and < 135:
                    orientation = Orientation.PageRight;
                    break;
                case >= 135 and < 225:
                    orientation = Orientation.PageDown;
                    break;
                case >= 225 and < 315:
                    orientation = Orientation.PageLeft;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation));
            }
        }

        private static TesseractOCR.Pix.Image LoadTestImage(string path)
        {
            var fullExampleImagePath = TestFilePath(path);
            return TesseractOCR.Pix.Image.LoadFromFile(fullExampleImagePath);
        }
        #endregion
    }
}