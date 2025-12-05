using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string _coverFramesFolder = @"E:\Random Project\Attack\Cover\"; //contains only image with same name as stego folder's files
            string _stegoFramesFolder = @"E:\Random Project\Attack\Stego\"; //contains only image with same name as cover folder's files
            string _outputResultFolder = @"E:\Random Project\Attack\AttackOutput"; //contains empty dir and folder

            #region Fields
            double SumBER = 0;
            double SumNCC = 0;
            double SumZNCC = 0;
            double SumNLSC_Cover = 0;
            double SumNLSC_Stego = 0;

            double ChiSquaredSumBER = 0;
            double ChiSquaredSumNCC = 0;
            double ChiSquaredSumZNCC = 0;
            double ChiSquaredSumNLSC_Cover = 0;
            double ChiSquaredSumNLSC_Stego = 0;

            double VisualAttackSumBER = 0;
            double VisualAttackSumNCC = 0;
            double VisualAttackSumZNCC = 0;
            double VisualAttackSumNLSC_Cover = 0;
            double VisualAttackSumNLSC_Stego = 0;

            double StatisticalAttackSumBER = 0;
            double StatisticalAttackSumNCC = 0;
            double StatisticalAttackSumZNCC = 0;
            double StatisticalAttackSumNLSC_Cover = 0;
            double StatisticalAttackSumNLSC_Stego = 0;

            double StructuralAttackSumBER = 0;
            double StructuralAttackSumNCC = 0;
            double StructuralAttackSumZNCC = 0;
            double StructuralAttackSumNLSC_Cover = 0;
            double StructuralAttackSumNLSC_Stego = 0;

            double CompressionAttackSumBER = 0;
            double CompressionAttackSumNCC = 0;
            double CompressionAttackSumZNCC = 0;
            double CompressionAttackSumNLSC_Cover = 0;
            double CompressionAttackSumNLSC_Stego = 0;

            double SignalProcessingAttackSumBER = 0;
            double SignalProcessingAttackSumNCC = 0;
            double SignalProcessingAttackSumZNCC = 0;
            double SignalProcessingAttackSumNLSC_Cover = 0;
            double SignalProcessingAttackSumNLSC_Stego = 0;

            double GeometricalAttackSumRotatedImageBer = 0, GeometricalAttackSumRotatedImageNCC = 0, GeometricalAttackSumRotatedImageZNCC = 0, GeometricalAttackSumRotatedImageNLSE_Cover = 0, GeometricalAttackSumRotatedImageNLSE_Stego = 0,
                GeometricalAttackSumScaledImageBer = 0, GeometricalAttackSumScaledImageNCC = 0, GeometricalAttackSumScaledImageZNCC = 0, GeometricalAttackSumScaledImageNLSE_Cover = 0, GeometricalAttackSumScaledImageNLSE_Stego = 0,
                GeometricalAttackSumTranslatedImageBer = 0, GeometricalAttackSumTranslatedImageNCC = 0, GeometricalAttackSumTranslatedImageZNCC = 0, GeometricalAttackSumTranslatedImageNLSE_Cover = 0, GeometricalAttackSumTranslatedImageNLSE_Stego = 0,
                GeometricalAttackSumFlippedImageBer = 0, GeometricalAttackSumFlippedImageNCC = 0, GeometricalAttackSumFlippedImageZNCC = 0, GeometricalAttackSumFlippedImageNLSE_Cover = 0, GeometricalAttackSumFlippedImageNLSE_Stego = 0;

            double SaltAndPepperNoiseAttackSumBER = 0;
            double SaltAndPepperNoiseAttackSumNCC = 0;
            double SaltAndPepperNoiseAttackSumZNCC = 0;
            double SaltAndPepperNoiseAttackSumNLSC_Cover = 0;
            double SaltAndPepperNoiseAttackSumNLSC_Stego = 0;

            double SpeckleNoiseAttackSumBER = 0;
            double SpeckleNoiseAttackSumNCC = 0;
            double SpeckleNoiseAttackSumZNCC = 0;
            double SpeckleNoiseAttackSumNLSC_Cover = 0;
            double SpeckleNoiseAttackSumNLSC_Stego = 0;

            double MedianFilterAttackSumBER = 0;
            double MedianFilterAttackSumNCC = 0;
            double MedianFilterAttackSumZNCC = 0;
            double MedianFilterAttackSumNLSC_Cover = 0;
            double MedianFilterAttackSumNLSC_Stego = 0;

            string _averageLog = "";
            #endregion

            Console.WriteLine("Start!");
            int totalFrames = Directory.GetFiles(_stegoFramesFolder).Count();
            for (int i = 0; i < totalFrames; i++)
            {
                string _log = "";
                string frameName = Convert.ToInt16(i + 1).ToString("D6");

                Console.WriteLine($"Process Start for Frame Number: {frameName}>>>>>>>>>>>>>>>>>");

                Bitmap coverImg = new Bitmap(_coverFramesFolder + @"\" + (frameName + ".bmp"));
                Bitmap stegoImg = new Bitmap(_stegoFramesFolder + @"\" + (frameName + ".bmp"));


                string outputFolderForEachFrame = _outputResultFolder + @"\" + frameName;
                if (!Directory.Exists(outputFolderForEachFrame))
                    Directory.CreateDirectory(outputFolderForEachFrame);

                #region Cover and Stego Image BER, NCC, ZNCC, NLSE

                Console.WriteLine($"Log is generating for Robustness results analysis");

                coverImg.Save(outputFolderForEachFrame + @"\" + frameName + "_cover.png");
                stegoImg.Save(outputFolderForEachFrame + @"\" + frameName + "_stego.png");

                double originalBER = Attacks.CalculateBER(coverImg, stegoImg);
                double originalNCC = Attacks.CalculateNCC(coverImg, stegoImg);
                double originalZNCC = Attacks.CalculateZNCC(coverImg, stegoImg);
                (double, double) originalNLSE = Attacks.CalculateNLSE(coverImg, stegoImg);

                SumBER += originalBER;
                SumNCC += originalNCC;
                SumZNCC += originalZNCC;
                SumNLSC_Cover += originalNLSE.Item1;
                SumNLSC_Stego += originalNLSE.Item2;

                _log += $"BER for Cover & Stego Image: {originalBER} {Environment.NewLine}";
                _log += $"NCC for Cover & Stego Image: {originalNCC} {Environment.NewLine}";
                _log += $"ZNCC for Cover & Stego Image: {originalZNCC} {Environment.NewLine}";
                _log += $"NLSE for Cover Image: {originalNLSE.Item1} {Environment.NewLine}";
                _log += $"NLSE for Stego Image: {originalNLSE.Item2} {Environment.NewLine}";

                double origianalChiSquareValue = Attacks.ChiSquareAttackSimilarityResult(coverImg, stegoImg);

                // Determine if steganography is detected based on threshold
                if (origianalChiSquareValue > 0)
                    _log += $"Steganography Is Detected {Environment.NewLine}";
                else
                    _log += $"Steganography Is Not Detected {Environment.NewLine}";

                File.WriteAllText(outputFolderForEachFrame + @"\Log for Robustness results analysis.txt", _log);

                #endregion

                #region Chi Squared Attack

                Console.WriteLine($"Starting Chi Squared Attack");

                string ChiSquaredAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Chi Squared Attack";
                if (!Directory.Exists(ChiSquaredAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(ChiSquaredAttackOutputFolderForEachFrame);

                string _logChiSquaredAttack = "";

                Bitmap coverImgChiSquaredAttackImg = Attacks.ApplyChiSquareAttack(coverImg);
                Bitmap stegoImgChiSquaredAttackImg = Attacks.ApplyChiSquareAttack(stegoImg);

                coverImgChiSquaredAttackImg.Save(ChiSquaredAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgChiSquaredAttackImg.png");
                stegoImgChiSquaredAttackImg.Save(ChiSquaredAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgChiSquaredAttackImg.png");

                double attackedChiSquareValue = Attacks.ChiSquareAttackSimilarityResult(coverImgChiSquaredAttackImg, stegoImgChiSquaredAttackImg);

                // Set a threshold for detecting changes (adjust as needed)
                double threshold = 0; // You need to find an appropriate threshold based on your images

                _logChiSquaredAttack += $"Original Images Similarity Value: {attackedChiSquareValue} {Environment.NewLine}";
                _logChiSquaredAttack += $"Attacked Images Similarity Value: {attackedChiSquareValue} {Environment.NewLine}";

                // Determine if steganography is detected based on threshold
                if (attackedChiSquareValue > threshold)
                    _logChiSquaredAttack += $"Attacked->Steganography Is Detected {Environment.NewLine}";
                else
                    _logChiSquaredAttack += $"Attacked->Steganography Is Not Detected {Environment.NewLine}";

                _logChiSquaredAttack += $"{Environment.NewLine}";

                double ChiSquaredAttackBER = Attacks.CalculateBER(coverImgChiSquaredAttackImg, stegoImgChiSquaredAttackImg);
                double ChiSquaredAttackNCC = Attacks.CalculateNCC(coverImgChiSquaredAttackImg, stegoImgChiSquaredAttackImg);
                double ChiSquaredAttackZNCC = Attacks.CalculateZNCC(coverImgChiSquaredAttackImg, stegoImgChiSquaredAttackImg);
                (double, double) ChiSquaredAttackNLSE = Attacks.CalculateNLSE(coverImgChiSquaredAttackImg, stegoImgChiSquaredAttackImg);

                ChiSquaredSumBER += ChiSquaredAttackBER;
                ChiSquaredSumNCC += ChiSquaredAttackNCC;
                ChiSquaredSumZNCC += ChiSquaredAttackZNCC;
                ChiSquaredSumNLSC_Cover += ChiSquaredAttackNLSE.Item1;
                ChiSquaredSumNLSC_Stego += ChiSquaredAttackNLSE.Item2;

                _logChiSquaredAttack += $"BER for Cover & Stego Chi Squared Attack Image: {ChiSquaredAttackBER} {Environment.NewLine}";
                _logChiSquaredAttack += $"NCC for Cover & Stego Chi Squared Attack Image: {ChiSquaredAttackNCC} {Environment.NewLine}";
                _logChiSquaredAttack += $"ZNCC for Cover & Stego Chi Squared Attack Image: {ChiSquaredAttackZNCC} {Environment.NewLine}";
                _logChiSquaredAttack += $"NLSE for Cover Chi Squared Attack Image: {ChiSquaredAttackNLSE.Item1} {Environment.NewLine}";
                _logChiSquaredAttack += $"NLSE for Stego Chi Squared Attack Image: {ChiSquaredAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(ChiSquaredAttackOutputFolderForEachFrame + @"\Log_Chi_Squared_Attack.txt", _logChiSquaredAttack);

                Console.WriteLine($"Finish Chi Squared Attack");

                #endregion

                #region Visual Attack

                Console.WriteLine($"Start Visual Attack");

                string VisualAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Visual Attack";
                if (!Directory.Exists(VisualAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(VisualAttackOutputFolderForEachFrame);

                string _logVisualAttack = "";

                Bitmap coverImgVisualAttackImg = Attacks.ApplyVisualAttack(coverImg);
                Bitmap stegoImgVisualAttackImg = Attacks.ApplyVisualAttack(stegoImg);

                coverImgVisualAttackImg.Save(VisualAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgVisualAttackImg.png");
                stegoImgVisualAttackImg.Save(VisualAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgVisualAttackImg.png");

                bool areImagesVisualAttackIdentical = Attacks.VisualAttackIdentity(coverImgVisualAttackImg, stegoImgVisualAttackImg);

                if (areImagesVisualAttackIdentical)
                    _logVisualAttack += $"Images are identical. No steganography detected. {Environment.NewLine}";
                else
                    _logVisualAttack += $"Images differ. Possible steganography detected. {Environment.NewLine}";

                double VisualAttackBER = Attacks.CalculateBER(coverImgVisualAttackImg, stegoImgVisualAttackImg);
                double VisualAttackNCC = Attacks.CalculateNCC(coverImgVisualAttackImg, stegoImgVisualAttackImg);
                double VisualAttackZNCC = Attacks.CalculateZNCC(coverImgVisualAttackImg, stegoImgVisualAttackImg);
                (double, double) VisualAttackNLSE = Attacks.CalculateNLSE(coverImgVisualAttackImg, stegoImgVisualAttackImg);

                VisualAttackSumBER += VisualAttackBER;
                VisualAttackSumNCC += VisualAttackNCC;
                VisualAttackSumZNCC += VisualAttackZNCC;
                VisualAttackSumNLSC_Cover += VisualAttackNLSE.Item1;
                VisualAttackSumNLSC_Stego += VisualAttackNLSE.Item2;

                _logVisualAttack += $"BER for Cover & Stego Visual Attack Image: {VisualAttackBER} {Environment.NewLine}";
                _logVisualAttack += $"NCC for Cover & Stego Visual Attack Image: {VisualAttackNCC} {Environment.NewLine}";
                _logVisualAttack += $"ZNCC for Cover & Stego Visual Attack Image: {VisualAttackZNCC} {Environment.NewLine}";
                _logVisualAttack += $"NLSE for Cover Visual Attack Image: {VisualAttackNLSE.Item1} {Environment.NewLine}";
                _logVisualAttack += $"NLSE for Stego Visual Attack Image: {VisualAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(VisualAttackOutputFolderForEachFrame + @"\Log_Visual_Attack.txt", _logVisualAttack);

                Console.WriteLine($"End Visual Attack");

                #endregion

                #region Statistical Attack

                Console.WriteLine($"Start Statistical Attack");

                string StatisticalAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Statistical Attack";
                if (!Directory.Exists(StatisticalAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(StatisticalAttackOutputFolderForEachFrame);

                string _logStatisticalAttack = "";

                Bitmap coverImgStatisticalAttackImg = Attacks.ApplyStatisticalAttack(coverImg);
                Bitmap stegoImgStatisticalAttackImg = Attacks.ApplyStatisticalAttack(stegoImg);

                coverImgStatisticalAttackImg.Save(StatisticalAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgStatisticalAttackImg.png");
                stegoImgStatisticalAttackImg.Save(StatisticalAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgStatisticalAttackImg.png");

                (double, double) areImagesStatisticalAttackIdentical = Attacks.StatisticalAttackIdentity(coverImgStatisticalAttackImg, stegoImgStatisticalAttackImg);

                _logStatisticalAttack += $"Mean: {areImagesStatisticalAttackIdentical.Item1}. {Environment.NewLine}";
                _logStatisticalAttack += $"Standard deviation: {areImagesStatisticalAttackIdentical.Item2}. {Environment.NewLine}";

                // Set thresholds for mean and standard deviation differences (adjust as needed)
                double meanThreshold = 0;
                double stdDevThreshold = 0;

                // If mean and std deviation differences are below thresholds, images are considered similar
                if (areImagesStatisticalAttackIdentical.Item1 < meanThreshold && areImagesStatisticalAttackIdentical.Item2 < stdDevThreshold)
                    _logStatisticalAttack += $"Images are identical. No steganography detected. {Environment.NewLine}";
                else
                    _logStatisticalAttack += $"Images differ. Possible steganography detected. {Environment.NewLine}";


                double StatisticalAttackBER = Attacks.CalculateBER(coverImgStatisticalAttackImg, stegoImgStatisticalAttackImg);
                double StatisticalAttackNCC = Attacks.CalculateNCC(coverImgStatisticalAttackImg, stegoImgStatisticalAttackImg);
                double StatisticalAttackZNCC = Attacks.CalculateZNCC(coverImgStatisticalAttackImg, stegoImgStatisticalAttackImg);
                (double, double) StatisticalAttackNLSE = Attacks.CalculateNLSE(coverImgStatisticalAttackImg, stegoImgStatisticalAttackImg);

                StatisticalAttackSumBER += StatisticalAttackBER;
                StatisticalAttackSumNCC += StatisticalAttackNCC;
                StatisticalAttackSumZNCC += StatisticalAttackZNCC;
                StatisticalAttackSumNLSC_Cover += StatisticalAttackNLSE.Item1;
                StatisticalAttackSumNLSC_Stego += StatisticalAttackNLSE.Item2;

                _logStatisticalAttack += $"BER for Cover & Stego Statistical Attack Image: {StatisticalAttackBER} {Environment.NewLine}";
                _logStatisticalAttack += $"NCC for Cover & Stego Statistical Attack Image: {StatisticalAttackNCC} {Environment.NewLine}";
                _logStatisticalAttack += $"ZNCC for Cover & Stego Statistical Attack Image: {StatisticalAttackZNCC} {Environment.NewLine}";
                _logStatisticalAttack += $"NLSE for Cover Statistical Attack Image: {StatisticalAttackNLSE.Item1} {Environment.NewLine}";
                _logStatisticalAttack += $"NLSE for Stego Statistical Attack Image: {StatisticalAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(StatisticalAttackOutputFolderForEachFrame + @"\Log__Statistical_Attack.txt", _logStatisticalAttack);

                Console.WriteLine($"End Statistical Attack");
                #endregion

                #region Structural Attack

                Console.WriteLine($"Start Structural Attack");

                string StructuralAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Structural Attack";
                if (!Directory.Exists(StructuralAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(StructuralAttackOutputFolderForEachFrame);

                string _logStructuralAttack = "";

                Bitmap coverImgStructuralAttackImg = Attacks.StructuralAttack(coverImg);
                Bitmap stegoImgStructuralAttackImg = Attacks.StructuralAttack(stegoImg);

                coverImgStructuralAttackImg.Save(StructuralAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgStructuralAttackImg.png");
                stegoImgStructuralAttackImg.Save(StructuralAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgStructuralAttackImg.png");

                // Compare edge maps
                double similarity = Attacks.CalculateStructuralSimilarity(coverImgStructuralAttackImg, stegoImgStructuralAttackImg);

                _logStructuralAttack += $"Similarity: {similarity} {Environment.NewLine}";

                // Set a threshold for structural similarity (adjust as needed)
                double structuralThreshold = 10;

                if (similarity >= structuralThreshold)
                    _logStructuralAttack += $"Image structures are similar. No steganography detected. {Environment.NewLine}";
                else
                    _logStructuralAttack += $"Image structures are different. Possible steganography detected. {Environment.NewLine}";



                double StructuralAttackBER = Attacks.CalculateBER(coverImgStructuralAttackImg, stegoImgStructuralAttackImg);
                double StructuralAttackNCC = Attacks.CalculateNCC(coverImgStructuralAttackImg, stegoImgStructuralAttackImg);
                double StructuralAttackZNCC = Attacks.CalculateZNCC(coverImgStructuralAttackImg, stegoImgStructuralAttackImg);
                (double, double) StructuralAttackNLSE = Attacks.CalculateNLSE(coverImgStructuralAttackImg, stegoImgStructuralAttackImg);

                StructuralAttackSumBER += StructuralAttackBER;
                StructuralAttackSumNCC += StructuralAttackNCC;
                StructuralAttackSumZNCC += StructuralAttackZNCC;
                StructuralAttackSumNLSC_Cover += StructuralAttackNLSE.Item1;
                StructuralAttackSumNLSC_Stego += StructuralAttackNLSE.Item2;

                _logStructuralAttack += $"BER for Cover & Stego Structural Attack Image: {StructuralAttackBER} {Environment.NewLine}";
                _logStructuralAttack += $"NCC for Cover & Stego Structural Attack Image: {StructuralAttackNCC} {Environment.NewLine}";
                _logStructuralAttack += $"ZNCC for Cover & Stego Structural Attack Image: {StructuralAttackZNCC} {Environment.NewLine}";
                _logStructuralAttack += $"NLSE for Cover Structural Attack Image: {StructuralAttackNLSE.Item1} {Environment.NewLine}";
                _logStructuralAttack += $"NLSE for Stego Structural Attack Image: {StructuralAttackNLSE.Item2} {Environment.NewLine}";



                File.WriteAllText(StructuralAttackOutputFolderForEachFrame + @"\Log__Structural_Attack.txt", _logStructuralAttack);

                Console.WriteLine($"End Structural Attack");
                #endregion

                #region Compression Attack

                Console.WriteLine($"Start Compression Attack");

                string CompressionAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Compression Attack";
                if (!Directory.Exists(CompressionAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(CompressionAttackOutputFolderForEachFrame);

                string _logCompressionAttack = "";

                var coverImgCompressionAttackImgParam = Attacks.ApplyCompressionAttack(coverImg);
                var stegoImgCompressionAttackImgParam = Attacks.ApplyCompressionAttack(stegoImg);

                coverImg.Save(CompressionAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgCompressionAttackImg.png", coverImgCompressionAttackImgParam.Item1, coverImgCompressionAttackImgParam.Item2);
                stegoImg.Save(CompressionAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgCompressionAttackImg.png", stegoImgCompressionAttackImgParam.Item1, stegoImgCompressionAttackImgParam.Item2);

                Bitmap coverImgCompressionAttackImg = new Bitmap(CompressionAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgCompressionAttackImg.png");
                Bitmap stegoImgCompressionAttackImg = new Bitmap(CompressionAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgCompressionAttackImg.png");

                double CompressionAttackBER = Attacks.CalculateBER(coverImgCompressionAttackImg, stegoImgCompressionAttackImg);
                double CompressionAttackNCC = Attacks.CalculateNCC(coverImgCompressionAttackImg, stegoImgCompressionAttackImg);
                double CompressionAttackZNCC = Attacks.CalculateZNCC(coverImgCompressionAttackImg, stegoImgCompressionAttackImg);
                (double, double) CompressionAttackNLSE = Attacks.CalculateNLSE(coverImgCompressionAttackImg, stegoImgCompressionAttackImg);

                CompressionAttackSumBER += CompressionAttackBER;
                CompressionAttackSumNCC += CompressionAttackNCC;
                CompressionAttackSumZNCC += CompressionAttackZNCC;
                CompressionAttackSumNLSC_Cover += CompressionAttackNLSE.Item1;
                CompressionAttackSumNLSC_Stego += CompressionAttackNLSE.Item2;

                _logCompressionAttack += $"BER for Cover & Stego Compression Attack Image: {CompressionAttackBER} {Environment.NewLine}";
                _logCompressionAttack += $"NCC for Cover & Stego Compression Attack Image: {CompressionAttackNCC} {Environment.NewLine}";
                _logCompressionAttack += $"ZNCC for Cover & Stego Compression Attack Image: {CompressionAttackZNCC} {Environment.NewLine}";
                _logCompressionAttack += $"NLSE for Cover Compression Attack Image: {CompressionAttackNLSE.Item1} {Environment.NewLine}";
                _logCompressionAttack += $"NLSE for Stego Compression Attack Image: {CompressionAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(CompressionAttackOutputFolderForEachFrame + @"\Log_Compression_Attack.txt", _logCompressionAttack);

                Console.WriteLine($"End Compression Attack");

                #endregion

                #region Geometrical Attack

                Console.WriteLine($"Start Geometrical Attack");

                string GeometricalAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Geometrical Attack";
                if (!Directory.Exists(GeometricalAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(GeometricalAttackOutputFolderForEachFrame);

                string _logGeometricalAttack = "";

                var data = Attacks.ApplyGeometricalAttack(coverImg, stegoImg, GeometricalAttackOutputFolderForEachFrame);
                _logGeometricalAttack += data.Item1;


                GeometricalAttackSumRotatedImageBer += data.Item2;
                GeometricalAttackSumRotatedImageNCC += data.Item3;
                GeometricalAttackSumRotatedImageZNCC += data.Item4;
                GeometricalAttackSumRotatedImageNLSE_Cover += data.Item5;
                GeometricalAttackSumRotatedImageNLSE_Stego += data.Item6;
                GeometricalAttackSumScaledImageBer += data.Item7;
                GeometricalAttackSumScaledImageNCC += data.Item8;
                GeometricalAttackSumScaledImageZNCC += data.Item9;
                GeometricalAttackSumScaledImageNLSE_Cover += data.Item10;
                GeometricalAttackSumScaledImageNLSE_Stego += data.Item11;
                GeometricalAttackSumTranslatedImageBer += data.Item12;
                GeometricalAttackSumTranslatedImageNCC += data.Item13;
                GeometricalAttackSumTranslatedImageZNCC += data.Item14;
                GeometricalAttackSumTranslatedImageNLSE_Cover += data.Item15;
                GeometricalAttackSumTranslatedImageNLSE_Stego += data.Item16;
                GeometricalAttackSumFlippedImageBer += data.Item17;
                GeometricalAttackSumFlippedImageNCC += data.Item18;
                GeometricalAttackSumFlippedImageZNCC += data.Item19;
                GeometricalAttackSumFlippedImageNLSE_Cover += data.Item20;
                GeometricalAttackSumFlippedImageNLSE_Stego += data.Item21;

                File.WriteAllText(GeometricalAttackOutputFolderForEachFrame + @"\Log_Geometrical_Attack.txt", _logGeometricalAttack);

                Console.WriteLine($"End Geometrical Attack");
                #endregion

                #region SignalProcessing attack

                Console.WriteLine($"Start SignalProcessing Attack");

                string SignalProcessingAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Signal Processing Attack";
                if (!Directory.Exists(SignalProcessingAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(SignalProcessingAttackOutputFolderForEachFrame);

                string _logSignalProcessingAttack = "";

                var coverImgSignalProcessingAttackImg = Attacks.ApplySignalProcessingAttack(coverImg);
                var stegoImgSignalProcessingAttackImg = Attacks.ApplySignalProcessingAttack(stegoImg);

                coverImgSignalProcessingAttackImg.Save(SignalProcessingAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgSignalProcessingAttackImg.png");
                stegoImgSignalProcessingAttackImg.Save(SignalProcessingAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgSignalProcessingAttackImg.png");


                double SignalProcessingAttackBER = Attacks.CalculateBER(coverImgSignalProcessingAttackImg, stegoImgSignalProcessingAttackImg);
                double SignalProcessingAttackNCC = Attacks.CalculateNCC(coverImgSignalProcessingAttackImg, stegoImgSignalProcessingAttackImg);
                double SignalProcessingAttackZNCC = Attacks.CalculateZNCC(coverImgSignalProcessingAttackImg, stegoImgSignalProcessingAttackImg);
                (double, double) SignalProcessingAttackNLSE = Attacks.CalculateNLSE(coverImgSignalProcessingAttackImg, stegoImgSignalProcessingAttackImg);

                SignalProcessingAttackSumBER += SignalProcessingAttackBER;
                SignalProcessingAttackSumNCC += SignalProcessingAttackNCC;
                SignalProcessingAttackSumZNCC += SignalProcessingAttackZNCC;
                SignalProcessingAttackSumNLSC_Cover += SignalProcessingAttackNLSE.Item1;
                SignalProcessingAttackSumNLSC_Stego += SignalProcessingAttackNLSE.Item2;

                _logSignalProcessingAttack += $"BER for Cover & Stego SignalProcessing Attack Image: {SignalProcessingAttackBER} {Environment.NewLine}";
                _logSignalProcessingAttack += $"NCC for Cover & Stego SignalProcessing Attack Image: {SignalProcessingAttackNCC} {Environment.NewLine}";
                _logSignalProcessingAttack += $"ZNCC for Cover & Stego SignalProcessing Attack Image: {SignalProcessingAttackZNCC} {Environment.NewLine}";
                _logSignalProcessingAttack += $"NLSE for Cover SignalProcessing Attack Image: {SignalProcessingAttackNLSE.Item1} {Environment.NewLine}";
                _logSignalProcessingAttack += $"NLSE for Stego SignalProcessing Attack Image: {SignalProcessingAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(SignalProcessingAttackOutputFolderForEachFrame + @"\Log_SignalProcessing_Attack.txt", _logSignalProcessingAttack);

                Console.WriteLine($"End SignalProcessing Attack");
                #endregion

                #region SaltAndPepperNoise Attack

                Console.WriteLine($"Start SaltAndPepperNoise Attack");

                string SaltAndPepperNoiseAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Salt And Pepper Noise Attack";
                if (!Directory.Exists(SaltAndPepperNoiseAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(SaltAndPepperNoiseAttackOutputFolderForEachFrame);

                string _logSaltAndPepperNoiseAttack = "";

                var coverImgSaltAndPepperNoiseAttackImg = Attacks.ApplySaltAndPepperNoiseAttack(coverImg);
                var stegoImgSaltAndPepperNoiseAttackImg = Attacks.ApplySaltAndPepperNoiseAttack(stegoImg);

                coverImgSaltAndPepperNoiseAttackImg.Save(SaltAndPepperNoiseAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgSaltAndPepperNoiseAttackImg.png");
                stegoImgSaltAndPepperNoiseAttackImg.Save(SaltAndPepperNoiseAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgSaltAndPepperNoiseAttackImg.png");


                double SaltAndPepperNoiseAttackBER = Attacks.CalculateBER(coverImgSaltAndPepperNoiseAttackImg, stegoImgSaltAndPepperNoiseAttackImg);
                double SaltAndPepperNoiseAttackNCC = Attacks.CalculateNCC(coverImgSaltAndPepperNoiseAttackImg, stegoImgSaltAndPepperNoiseAttackImg);
                double SaltAndPepperNoiseAttackZNCC = Attacks.CalculateZNCC(coverImgSaltAndPepperNoiseAttackImg, stegoImgSaltAndPepperNoiseAttackImg);
                (double, double) SaltAndPepperNoiseAttackNLSE = Attacks.CalculateNLSE(coverImgSaltAndPepperNoiseAttackImg, stegoImgSaltAndPepperNoiseAttackImg);

                SaltAndPepperNoiseAttackSumBER += SaltAndPepperNoiseAttackBER;
                SaltAndPepperNoiseAttackSumNCC += SaltAndPepperNoiseAttackNCC;
                SaltAndPepperNoiseAttackSumZNCC += SaltAndPepperNoiseAttackZNCC;
                SaltAndPepperNoiseAttackSumNLSC_Cover += SaltAndPepperNoiseAttackNLSE.Item1;
                SaltAndPepperNoiseAttackSumNLSC_Stego += SaltAndPepperNoiseAttackNLSE.Item2;

                _logSaltAndPepperNoiseAttack += $"BER for Cover & Stego SaltAndPepperNoise Attack Image: {SaltAndPepperNoiseAttackBER} {Environment.NewLine}";
                _logSaltAndPepperNoiseAttack += $"NCC for Cover & Stego SaltAndPepperNoise Attack Image: {SaltAndPepperNoiseAttackNCC} {Environment.NewLine}";
                _logSaltAndPepperNoiseAttack += $"ZNCC for Cover & Stego SaltAndPepperNoise Attack Image: {SaltAndPepperNoiseAttackZNCC} {Environment.NewLine}";
                _logSaltAndPepperNoiseAttack += $"NLSE for Cover SaltAndPepperNoise Attack Image: {SaltAndPepperNoiseAttackNLSE.Item1} {Environment.NewLine}";
                _logSaltAndPepperNoiseAttack += $"NLSE for Stego SaltAndPepperNoise Attack Image: {SaltAndPepperNoiseAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(SaltAndPepperNoiseAttackOutputFolderForEachFrame + @"\Log_SaltAndPepperNoise_Attack.txt", _logSaltAndPepperNoiseAttack);

                Console.WriteLine($"End SaltAndPepperNoise Attack");
                #endregion

                #region SpeckleNoise Attack

                Console.WriteLine($"Start SpeckleNoise Attack");

                string SpeckleNoiseAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Speckle Noise Attack";
                if (!Directory.Exists(SpeckleNoiseAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(SpeckleNoiseAttackOutputFolderForEachFrame);

                string _logSpeckleNoiseAttack = "";

                var coverImgSpeckleNoiseAttackImg = Attacks.ApplySpeckleNoiseAttack(coverImg);
                var stegoImgSpeckleNoiseAttackImg = Attacks.ApplySpeckleNoiseAttack(stegoImg);

                coverImgSpeckleNoiseAttackImg.Save(SpeckleNoiseAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgSpeckleNoiseAttackImg.png");
                stegoImgSpeckleNoiseAttackImg.Save(SpeckleNoiseAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgSpeckleNoiseAttackImg.png");


                double SpeckleNoiseAttackBER = Attacks.CalculateBER(coverImgSpeckleNoiseAttackImg, stegoImgSpeckleNoiseAttackImg);
                double SpeckleNoiseAttackNCC = Attacks.CalculateNCC(coverImgSpeckleNoiseAttackImg, stegoImgSpeckleNoiseAttackImg);
                double SpeckleNoiseAttackZNCC = Attacks.CalculateZNCC(coverImgSpeckleNoiseAttackImg, stegoImgSpeckleNoiseAttackImg);
                (double, double) SpeckleNoiseAttackNLSE = Attacks.CalculateNLSE(coverImgSpeckleNoiseAttackImg, stegoImgSpeckleNoiseAttackImg);

                SpeckleNoiseAttackSumBER += SpeckleNoiseAttackBER;
                SpeckleNoiseAttackSumNCC += SpeckleNoiseAttackNCC;
                SpeckleNoiseAttackSumZNCC += SpeckleNoiseAttackZNCC;
                SpeckleNoiseAttackSumNLSC_Cover += SpeckleNoiseAttackNLSE.Item1;
                SpeckleNoiseAttackSumNLSC_Stego += SpeckleNoiseAttackNLSE.Item2;

                _logSpeckleNoiseAttack += $"BER for Cover & Stego SpeckleNoise Attack Image: {SpeckleNoiseAttackBER} {Environment.NewLine}";
                _logSpeckleNoiseAttack += $"NCC for Cover & Stego SpeckleNoise Attack Image: {SpeckleNoiseAttackNCC} {Environment.NewLine}";
                _logSpeckleNoiseAttack += $"ZNCC for Cover & Stego SpeckleNoise Attack Image: {SpeckleNoiseAttackZNCC} {Environment.NewLine}";
                _logSpeckleNoiseAttack += $"NLSE for Cover SpeckleNoise Attack Image: {SpeckleNoiseAttackNLSE.Item1} {Environment.NewLine}";
                _logSpeckleNoiseAttack += $"NLSE for Stego SpeckleNoise Attack Image: {SpeckleNoiseAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(SpeckleNoiseAttackOutputFolderForEachFrame + @"\Log_SpeckleNoise_Attack.txt", _logSpeckleNoiseAttack);

                Console.WriteLine($"End SpeckleNoise Attack");
                #endregion

                #region MedianFilter Attack

                Console.WriteLine($"Start MedianFilter Attack");

                string MedianFilterAttackOutputFolderForEachFrame = outputFolderForEachFrame + @"\Median Filter Attack";
                if (!Directory.Exists(MedianFilterAttackOutputFolderForEachFrame))
                    Directory.CreateDirectory(MedianFilterAttackOutputFolderForEachFrame);

                string _logMedianFilterAttack = "";

                var coverImgMedianFilterAttackImg = Attacks.ApplyMedianFilter(coverImg);
                var stegoImgMedianFilterAttackImg = Attacks.ApplyMedianFilter(stegoImg);

                coverImgMedianFilterAttackImg.Save(MedianFilterAttackOutputFolderForEachFrame + @"\" + frameName + "_coverImgMedianFilterAttackImg.png");
                stegoImgMedianFilterAttackImg.Save(MedianFilterAttackOutputFolderForEachFrame + @"\" + frameName + "_stegoImgMedianFilterAttackImg.png");


                double MedianFilterAttackBER = Attacks.CalculateBER(coverImgMedianFilterAttackImg, stegoImgMedianFilterAttackImg);
                double MedianFilterAttackNCC = Attacks.CalculateNCC(coverImgMedianFilterAttackImg, stegoImgMedianFilterAttackImg);
                double MedianFilterAttackZNCC = Attacks.CalculateZNCC(coverImgMedianFilterAttackImg, stegoImgMedianFilterAttackImg);
                (double, double) MedianFilterAttackNLSE = Attacks.CalculateNLSE(coverImgMedianFilterAttackImg, stegoImgMedianFilterAttackImg);

                MedianFilterAttackSumBER += MedianFilterAttackBER;
                MedianFilterAttackSumNCC += MedianFilterAttackNCC;
                MedianFilterAttackSumZNCC += MedianFilterAttackZNCC;
                MedianFilterAttackSumNLSC_Cover += MedianFilterAttackNLSE.Item1;
                MedianFilterAttackSumNLSC_Stego += MedianFilterAttackNLSE.Item2;

                _logMedianFilterAttack += $"BER for Cover & Stego  MedianFilter Attack Image: {MedianFilterAttackBER} {Environment.NewLine}";
                _logMedianFilterAttack += $"NCC for Cover & Stego  MedianFilter Attack Image: {MedianFilterAttackNCC} {Environment.NewLine}";
                _logMedianFilterAttack += $"ZNCC for Cover & Stego  MedianFilter Attack Image: {MedianFilterAttackZNCC} {Environment.NewLine}";
                _logMedianFilterAttack += $"NLSE for Cover  MedianFilter Attack Image: {MedianFilterAttackNLSE.Item1} {Environment.NewLine}";
                _logMedianFilterAttack += $"NLSE for Stego  MedianFilter Attack Image: {MedianFilterAttackNLSE.Item2} {Environment.NewLine}";

                File.WriteAllText(MedianFilterAttackOutputFolderForEachFrame + @"\Log_ MedianFilter_Attack.txt", _logMedianFilterAttack);

                Console.WriteLine($"End MedianFilter Attack");
                #endregion
            }

            #region Average Log

            _averageLog += $"Average BER for Cover & Stego Image: {SumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average NCC for Cover & Stego Image: {SumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average ZNCC for Cover & Stego Image: {SumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average NLSE for Cover Image: {SumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average NLSE for Stego Image: {SumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average ChiSquaredAttack BER for Cover & Stego Image: {ChiSquaredSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average ChiSquaredAttack NCC for Cover & Stego Image: {ChiSquaredSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average ChiSquaredAttack ZNCC for Cover & Stego Image: {ChiSquaredSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average ChiSquaredAttack NLSE for Cover Image: {ChiSquaredSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average ChiSquaredAttack NLSE for Stego Image: {ChiSquaredSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average VisualAttack BER for Cover & Stego Image: {VisualAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average VisualAttack NCC for Cover & Stego Image: {VisualAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average VisualAttack ZNCC for Cover & Stego Image: {VisualAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average VisualAttack NLSE for Cover Image: {VisualAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average VisualAttack NLSE for Stego Image: {VisualAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average StatisticalAttack BER for Cover & Stego Image: {StatisticalAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StatisticalAttack NCC for Cover & Stego Image: {StatisticalAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StatisticalAttack ZNCC for Cover & Stego Image: {StatisticalAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StatisticalAttack NLSE for Cover Image: {StatisticalAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StatisticalAttack NLSE for Stego Image: {StatisticalAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average StructuralAttack BER for Cover & Stego Image: {StructuralAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StructuralAttack NCC for Cover & Stego Image: {StructuralAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StructuralAttack ZNCC for Cover & Stego Image: {StructuralAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StructuralAttack NLSE for Cover Image: {StructuralAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average StructuralAttack NLSE for Stego Image: {StructuralAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average CompressionAttack BER for Cover & Stego Image: {CompressionAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average CompressionAttack NCC for Cover & Stego Image: {CompressionAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average CompressionAttack ZNCC for Cover & Stego Image: {CompressionAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average CompressionAttack NLSE for Cover Image: {CompressionAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average CompressionAttack NLSE for Stego Image: {CompressionAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average GeometricalAttack Rotated Image BER for Cover & Stego Image: {GeometricalAttackSumRotatedImageBer / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Rotated Image for NCC Cover & Stego Image: {GeometricalAttackSumRotatedImageNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Rotated Image for ZNCC Cover & Stego Image: {GeometricalAttackSumRotatedImageZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Rotated Image for NLSE Cover Image: {GeometricalAttackSumRotatedImageNLSE_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Rotated Image for NLSE Stego Image: {GeometricalAttackSumRotatedImageNLSE_Stego / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Scaled Image BER for Cover & Stego Image: {GeometricalAttackSumScaledImageBer / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Scaled Image for NCC Cover & Stego Image: {GeometricalAttackSumScaledImageNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Scaled Image for ZNCC Cover & Stego Image: {GeometricalAttackSumScaledImageZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Scaled Image for NLSE Cover Image: {GeometricalAttackSumScaledImageNLSE_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Scaled Image for NLSE Stego Image: {GeometricalAttackSumScaledImageNLSE_Stego / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Translated Image BER for Cover & Stego Image: {GeometricalAttackSumTranslatedImageBer / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Translated Image NCC for Cover & Stego Image: {GeometricalAttackSumTranslatedImageNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Translated Image ZNCC for Cover & Stego Image: {GeometricalAttackSumTranslatedImageZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Translated Image NLSE for Cover Image: {GeometricalAttackSumTranslatedImageNLSE_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Translated Image NLSE for Stego Image: {GeometricalAttackSumTranslatedImageNLSE_Stego / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Flipped Image BER for Cover & Stego Image: {GeometricalAttackSumFlippedImageBer / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Flipped Image NCC for Cover & Stego Image: {GeometricalAttackSumFlippedImageNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Flipped Image ZNCC for Cover & Stego Image: {GeometricalAttackSumFlippedImageZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Flipped Image NLSE for Cover Image: {GeometricalAttackSumFlippedImageNLSE_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average GeometricalAttack Flipped Image NLSE for Stego Image: {GeometricalAttackSumFlippedImageNLSE_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average SignalProcessingAttack BER for Cover & Stego Image: {SignalProcessingAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SignalProcessingAttack NCC for Cover & Stego Image: {SignalProcessingAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SignalProcessingAttack ZNCC for Cover & Stego Image: {SignalProcessingAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SignalProcessingAttack NLSE for Cover Image: {SignalProcessingAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SignalProcessingAttack NLSE for Stego Image: {SignalProcessingAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average SaltAndPepperNoise BER for Cover & Stego Image: {SaltAndPepperNoiseAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SaltAndPepperNoise NCC for Cover & Stego Image: {SaltAndPepperNoiseAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SaltAndPepperNoise ZNCC for Cover & Stego Image: {SaltAndPepperNoiseAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SaltAndPepperNoise NLSE for Cover Image: {SaltAndPepperNoiseAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SaltAndPepperNoise NLSE for Stego Image: {SaltAndPepperNoiseAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average SpeckleNoise BER for Cover & Stego Image: {SpeckleNoiseAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SpeckleNoise NCC for Cover & Stego Image: {SpeckleNoiseAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SpeckleNoise ZNCC for Cover & Stego Image: {SpeckleNoiseAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SpeckleNoise NLSE for Cover Image: {SpeckleNoiseAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average SpeckleNoise NLSE for Stego Image: {SpeckleNoiseAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";

            _averageLog += $"Average MedianFilter BER for Cover & Stego Image: {MedianFilterAttackSumBER / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average MedianFilter NCC for Cover & Stego Image: {MedianFilterAttackSumNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average MedianFilter ZNCC for Cover & Stego Image: {MedianFilterAttackSumZNCC / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average MedianFilter NLSE for Cover Image: {MedianFilterAttackSumNLSC_Cover / totalFrames} {Environment.NewLine}";
            _averageLog += $"Average MedianFilter NLSE for Stego Image: {MedianFilterAttackSumNLSC_Stego / totalFrames} {Environment.NewLine}{Environment.NewLine}";


            File.WriteAllText(_outputResultFolder + @"\Average Log for Robustness results analysis.txt", _averageLog);

            #endregion

            Console.WriteLine($"End>>>>>>>>>>>>>>>>>>>>>>");
            Console.ReadLine();
        }


    }
}
