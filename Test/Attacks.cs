using AForge.Imaging.Filters;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Test
{
    public class Attacks
    {

        #region ChiSquareAttack
        public static double ChiSquareAttackSimilarityResult(Bitmap originalImage, Bitmap stegoImage)
        {
            int numBins = 256; // Number of possible pixel values (0-255)
            int totalPixels = originalImage.Width * originalImage.Height;

            int[] originalHistogram = new int[numBins];
            int[] stegoHistogram = new int[numBins];

            // Calculate histograms for original and stego images
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color originalPixel = originalImage.GetPixel(x, y);
                    Color stegoPixel = stegoImage.GetPixel(x, y);

                    int grayscaleValueOP = (int)(originalPixel.R * 0.3 + originalPixel.G * 0.59 + originalPixel.B * 0.11); // Calculate grayscale value
                    int grayscaleValueSP = (int)(stegoPixel.R * 0.3 + stegoPixel.G * 0.59 + stegoPixel.B * 0.11); // Calculate grayscale value

                    originalHistogram[grayscaleValueOP]++;
                    stegoHistogram[grayscaleValueSP]++;
                }
            }

            // Calculate the Chi-Square value
            double chiSquareValue = 0.0;
            for (int i = 0; i < numBins; i++)
            {
                if (originalHistogram[i] > 0) // Avoid division by zero
                {
                    double expectedFrequency = (double)originalHistogram[i] / totalPixels;
                    double observedFrequency = (double)stegoHistogram[i] / totalPixels;

                    chiSquareValue += Math.Pow(observedFrequency - expectedFrequency, 2) / expectedFrequency;
                }
            }

            return chiSquareValue;
        }

        public static Bitmap ApplyChiSquareAttack(Bitmap image, double degreesOfFreedom = 2.0)
        {
            // Create a random number generator with a Chi-Square distribution
            Random random = new Random();

            // Create a new bitmap for the attacked image
            Bitmap attackedImage = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color originalColor = image.GetPixel(x, y);

                    // Apply Chi-Square distribution to each channel
                    int newRed = ApplyChiSquare(originalColor.R, degreesOfFreedom, random);
                    int newGreen = ApplyChiSquare(originalColor.G, degreesOfFreedom, random);
                    int newBlue = ApplyChiSquare(originalColor.B, degreesOfFreedom, random);

                    // Ensure pixel values are within the valid range [0, 255]
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    // Set the pixel value in the attacked image
                    attackedImage.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }

            return attackedImage;
        }

        private static int ApplyChiSquare(int originalValue, double degreesOfFreedom, Random random)
        {
            // Generate random value from a Chi-Square distribution
            double chiSquareValue = GenerateRandomChiSquare(degreesOfFreedom, random);

            // Apply the Chi-Square attack
            int attackedValue = (int)Math.Round(originalValue + chiSquareValue);

            return attackedValue;
        }

        private static double GenerateRandomChiSquare(double degreesOfFreedom, Random random)
        {
            // Generate random values from a standard normal distribution
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();

            // Box-Muller transform to generate two independent standard normal random variables
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            double z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // Square and sum to obtain a Chi-Square random variable
            double chiSquare = Math.Pow(z0, 2) + Math.Pow(z1, 2);

            // Scale by the degrees of freedom
            return chiSquare * degreesOfFreedom;
        }

        #endregion

        #region VisualAttack
        public static bool VisualAttackIdentity(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false; // Images have different dimensions, so they're not identical
            }

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    // Compare pixel values
                    if (pixel1 != pixel2)
                    {
                        return false; // Images differ at this pixel, so they're not identical
                    }
                }
            }

            return true; // All pixels are identical, so the images are the same
        }

        public static Bitmap ApplyVisualAttack(Bitmap image)
        {
            // Create a new bitmap for the attacked image
            Bitmap attackedImage = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color originalColor = image.GetPixel(x, y);

                    // Apply color inversion to each channel
                    int newRed = 255 - originalColor.R;
                    int newGreen = 255 - originalColor.G;
                    int newBlue = 255 - originalColor.B;

                    // Ensure pixel values are within the valid range [0, 255]
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    // Set the pixel value in the attacked image
                    attackedImage.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }

            return attackedImage;
        }
        #endregion

        #region StatisticalAttack
        public static (double, double) StatisticalAttackIdentity(Bitmap image1, Bitmap image2)
        {
            double mean1 = CalculateMeanIntensity(image1);
            double mean2 = CalculateMeanIntensity(image2);

            double stdDev1 = CalculateStandardDeviationIntensity(image1, mean1);
            double stdDev2 = CalculateStandardDeviationIntensity(image2, mean2);

            // Compare mean and standard deviation
            double meanDifference = Math.Abs(mean1 - mean2);
            double stdDevDifference = Math.Abs(stdDev1 - stdDev2);

            return (meanDifference, stdDevDifference);
        }

        private static double CalculateMeanIntensity(Bitmap image)
        {
            double totalIntensity = 0;
            int totalPixels = image.Width * image.Height;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    totalIntensity += (pixel.R + pixel.G + pixel.B) / 3.0; // Calculate average intensity
                }
            }

            return totalIntensity / totalPixels;
        }

        private static double CalculateStandardDeviationIntensity(Bitmap image, double mean)
        {
            double squaredDifferenceSum = 0;
            int totalPixels = image.Width * image.Height;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    double intensity = (pixel.R + pixel.G + pixel.B) / 3.0; // Calculate average intensity

                    squaredDifferenceSum += Math.Pow(intensity - mean, 2);
                }
            }

            double variance = squaredDifferenceSum / totalPixels;
            return Math.Sqrt(variance);
        }

        public static Bitmap ApplyStatisticalAttack(Bitmap image)
        {
            // Convert the image to grayscale
            Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);

            // Apply histogram equalization
            HistogramEqualization filter = new HistogramEqualization();
            return filter.Apply(grayImage);
        }
        #endregion

        #region StructuralAttack
        public static Bitmap StructuralAttack(Bitmap image1)
        {
            // Convert images to grayscale
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage1 = grayscaleFilter.Apply(image1);

            // Apply edge detection
            CannyEdgeDetector edgeDetector = new CannyEdgeDetector();
            Bitmap edgesImage1 = edgeDetector.Apply(grayImage1);

            return edgesImage1;
        }

        public static double CalculateStructuralSimilarity(Bitmap image1, Bitmap image2)
        {
            int totalPixels = image1.Width * image1.Height;
            int matchingPixels = 0;

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    double intensity1 = (pixel1.R + pixel1.G + pixel1.B) / 3.0;
                    double intensity2 = (pixel2.R + pixel2.G + pixel2.B) / 3.0;

                    if (Math.Abs(intensity1 - intensity2) < 20) // Adjust threshold as needed
                    {
                        matchingPixels++;
                    }
                }
            }

            return (double)matchingPixels / totalPixels;
        }
        #endregion

        #region CompressionAttack
        public static (ImageCodecInfo, EncoderParameters) ApplyCompressionAttack(Bitmap image, long compressionQuality = 50)
        {
            // Set up compression parameters
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, compressionQuality);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            // Get JPEG codec info
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/png");

            // Save the image with compression
            //image.Save(outputPath, jpegCodec, encoderParams);
            return (jpegCodec, encoderParams);
        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

        #region GeometricalAttack

        public static (string, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double)
            ApplyGeometricalAttack(Bitmap coverImage, Bitmap stegoImage, string outputFolder)
        {
            int rotateDgree = 30;
            double scale = 1.5;
            int translationX = 20;
            int translationY = 30;
            FlipType flipType = FlipType.Horizontal;

            Bitmap rotatedImageC = ApplyRotation(coverImage, rotateDgree); // Rotate by 30 degrees
            Bitmap scaledImageC = ApplyScaling(coverImage, scale, scale); // Scale by a factor of 1.5 in both dimensions
            Bitmap translatedImageC = ApplyTranslation(coverImage, translationX, translationY); // Translate by 20 pixels in the x-direction and 30 pixels in the y-direction
            Bitmap flippedImageC = ApplyFlip(coverImage, flipType); // Flip horizontally

            rotatedImageC.Save(outputFolder + @"\GeometricalAttackRotatedImageC.png");
            scaledImageC.Save(outputFolder + @"\GeometricalAttackScaledImageC.png");
            translatedImageC.Save(outputFolder + @"\GeometricalAttackTranslatedImageC.png");
            flippedImageC.Save(outputFolder + @"\GeometricalAttackFlippedImageC.png");

            Bitmap rotatedImageS = ApplyRotation(stegoImage, rotateDgree); // Rotate by 30 degrees
            Bitmap scaledImageS = ApplyScaling(stegoImage, scale, scale); // Scale by a factor of 1.5 in both dimensions
            Bitmap translatedImageS = ApplyTranslation(stegoImage, translationX, translationY); // Translate by 20 pixels in the x-direction and 30 pixels in the y-direction
            Bitmap flippedImageS = ApplyFlip(stegoImage, flipType); // Flip horizontally

            rotatedImageS.Save(outputFolder + @"\GeometricalAttackRotatedImageS.png");
            scaledImageS.Save(outputFolder + @"\GeometricalAttackScaledImageS.png");
            translatedImageS.Save(outputFolder + @"\GeometricalAttackTranslatedImageS.png");
            flippedImageS.Save(outputFolder + @"\GeometricalAttackFlippedImageS.png");

            double rotatedImageBer = CalculateBER(rotatedImageC, rotatedImageS);
            double scaledImageBer = CalculateBER(scaledImageC, scaledImageS);
            double translatedImageBer = CalculateBER(translatedImageC, translatedImageS);
            double flippedImageBer = CalculateBER(flippedImageC, flippedImageS);

            double rotatedImageNCC = CalculateNCC(rotatedImageC, rotatedImageS);
            double scaledImageNCC = CalculateNCC(scaledImageC, scaledImageS);
            double translatedImageNCC = CalculateNCC(translatedImageC, translatedImageS);
            double flippedImageNCC = CalculateNCC(flippedImageC, flippedImageS);

            double rotatedImageZNCC = CalculateNCC(rotatedImageC, rotatedImageS);
            double scaledImageZNCC = CalculateNCC(scaledImageC, scaledImageS);
            double translatedImageZNCC = CalculateNCC(translatedImageC, translatedImageS);
            double flippedImageZNCC = CalculateNCC(flippedImageC, flippedImageS);

            (double, double) rotatedImageNLSE = CalculateNLSE(rotatedImageC, rotatedImageS);
            (double, double) scaledImageNLSE = CalculateNLSE(scaledImageC, scaledImageS);
            (double, double) translatedImageNLSE = CalculateNLSE(translatedImageC, translatedImageS);
            (double, double) flippedImageNLSE = CalculateNLSE(flippedImageC, flippedImageS);

            #region String Result
            string result = $"ApplyCompressionAttack" + Environment.NewLine + Environment.NewLine +

                $"Rotated Image {rotateDgree} Degree BER: {rotatedImageBer}" + Environment.NewLine +
                $"Sacled Image {scale}x BER: {scaledImageBer}" + Environment.NewLine +
                $"Translated Image {translationX}X{translationY} BER: {translatedImageBer}" + Environment.NewLine +
                $"Flipped Image {flipType.ToString()} BER: {flippedImageBer}" + Environment.NewLine + Environment.NewLine +

                $"Rotated Image {rotateDgree} Degree NCC: {rotatedImageNCC}" + Environment.NewLine +
                $"Sacled Image {scale}x NCC: {scaledImageNCC}" + Environment.NewLine +
                $"Translated Image {translationX}X{translationY} NCC: {translatedImageNCC}" + Environment.NewLine +
                $"Flipped Image {flipType.ToString()} NCC: {flippedImageNCC}" + Environment.NewLine + Environment.NewLine +

                $"Rotated Image {rotateDgree} Degree ZNCC: {rotatedImageZNCC}" + Environment.NewLine +
                $"Sacled Image {scale}x ZNCC: {scaledImageZNCC}" + Environment.NewLine +
                $"Translated Image {translationX}X{translationY} ZNCC: {translatedImageZNCC}" + Environment.NewLine +
                $"Flipped Image {flipType.ToString()} ZNCC: {flippedImageZNCC}" + Environment.NewLine + Environment.NewLine +

                $"Rotated Image {rotateDgree} Degree NLSE: Cover Img: {rotatedImageNLSE.Item1} Stego Img: {rotatedImageNLSE.Item2}" + Environment.NewLine +
                $"Sacled Image {scale}x NLSE: Cover Img: {scaledImageNLSE.Item1} Stego Img: {scaledImageNLSE.Item2}" + Environment.NewLine +
                $"Translated Image {translationX}X{translationY} NLSE: Cover Img: {translatedImageNLSE.Item1} Stego Img: {translatedImageNLSE.Item2}" + Environment.NewLine +
                $"Flipped Image {flipType.ToString()} NLSE: Cover Img: {flippedImageNLSE.Item1} Stego Img: {flippedImageNLSE.Item2}" + Environment.NewLine + Environment.NewLine
                ;
            #endregion

            return (result,
                rotatedImageBer, rotatedImageNCC, rotatedImageZNCC, rotatedImageNLSE.Item1, rotatedImageNLSE.Item2,
                scaledImageBer, scaledImageNCC, scaledImageZNCC, scaledImageNLSE.Item1, scaledImageNLSE.Item2,
                translatedImageBer, translatedImageNCC, translatedImageZNCC, translatedImageNLSE.Item1, translatedImageNLSE.Item2,
                flippedImageBer, flippedImageNCC, flippedImageZNCC, flippedImageNLSE.Item1, flippedImageNLSE.Item2
               );

            Console.WriteLine("Geometrical attacks applied to the image.");
        }
        private static Bitmap ApplyRotation(Bitmap image, float angle)
        {
            // Apply rotation
            RotateBicubic rotateFilter = new RotateBicubic(angle, true);
            return rotateFilter.Apply(image);
        }
        private static Bitmap ApplyScaling(Bitmap image, double scaleX, double scaleY)
        {
            // Apply scaling
            ResizeBicubic resizeFilter = new ResizeBicubic((int)(image.Width * scaleX), (int)(image.Height * scaleY));
            return resizeFilter.Apply(image);
        }
        private static Bitmap ApplyTranslation(Bitmap image, int offsetX, int offsetY)
        {
            // Create a new bitmap with the same size as the original image
            Bitmap translatedImage = new Bitmap(image.Width, image.Height);

            // Create a Graphics object for drawing on the new bitmap
            using (Graphics g = Graphics.FromImage(translatedImage))
            {
                // Set the translation matrix
                g.TranslateTransform(offsetX, offsetY);

                // Draw the original image onto the new bitmap
                g.DrawImage(image, new Point(0, 0));
            }

            return translatedImage;
        }
        private static Bitmap ApplyFlip(Bitmap image, FlipType flipType)
        {
            // Apply flipping
            Mirror flipFilter = new Mirror(flipType == FlipType.Horizontal, flipType == FlipType.Vertical);
            return flipFilter.Apply(image);
        }
        #endregion

        #region signal processing attack
        public static Bitmap ApplySignalProcessingAttack(Bitmap coverImage)
        {
            // Apply signal processing attack (Gaussian blur) to both images
            Bitmap attackedImage = ApplyGaussianBlur(coverImage, 5); // Adjust the blur radius as needed

            return attackedImage;
        }

        private static Bitmap ApplyGaussianBlur(Bitmap image, float radius)
        {
            // Apply Gaussian blur
            GaussianBlur filter = new GaussianBlur(radius);
            return filter.Apply(image);
        }
        #endregion

        #region Salt And Pepper Noise Attack
        public static Bitmap ApplySaltAndPepperNoiseAttack(Bitmap image)
        {
            double noiseDensity = 0.05;

            Random random = new Random();
            Bitmap noisyImage = (Bitmap)image.Clone();

            for (int x = 0; x < noisyImage.Width; x++)
            {
                for (int y = 0; y < noisyImage.Height; y++)
                {
                    if (random.NextDouble() < noiseDensity)
                    {
                        // Randomly choose between black and white
                        Color newColor = random.NextDouble() < 0.5 ? Color.Black : Color.White;
                        noisyImage.SetPixel(x, y, newColor);
                    }
                }
            }

            return noisyImage;
        }
        #endregion

        #region Speckle Noise Attack
        public static Bitmap ApplySpeckleNoiseAttack(Bitmap image)
        {
            double noiseIntensity = 0.1;

            Random random = new Random();
            Bitmap noisyImage = (Bitmap)image.Clone();

            for (int x = 0; x < noisyImage.Width; x++)
            {
                for (int y = 0; y < noisyImage.Height; y++)
                {
                    Color originalColor = noisyImage.GetPixel(x, y);

                    // Generate random value
                    double noise = 1.0 + (random.NextDouble() - 0.5) * 2.0 * noiseIntensity;

                    // Multiply pixel values by the random value
                    int newRed = (int)(originalColor.R * noise);
                    int newGreen = (int)(originalColor.G * noise);
                    int newBlue = (int)(originalColor.B * noise);

                    // Ensure pixel values are within the valid range [0, 255]
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    // Set the pixel value in the noisy image
                    noisyImage.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }

            return noisyImage;
        }
        #endregion

        #region Median Filter Attack
        public static Bitmap ApplyMedianFilter(Bitmap image)
        {
            int filterSize = 3;
            // Create an instance of the Median filter
            Median filter = new Median(filterSize);

            // Apply the Median filter to the image
            return filter.Apply(image);
        }
        #endregion

        #region Robustness results analysis against attacks
        public static double CalculateBER(Bitmap image1, Bitmap image2)
        {
            if (image1.Size != image2.Size)
                throw new ArgumentException("Images must have the same size.");

            int errorCount = 0;
            int totalPixels = image1.Width * image1.Height;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    if (image1.GetPixel(x, y) != image2.GetPixel(x, y))
                    {
                        errorCount++;
                    }
                }
            }

            double ber = (double)errorCount / totalPixels;
            return ber;
        }

        public static (double, double) CalculateNLSE(Bitmap img1, Bitmap img2)
        {
            // Convert images to grayscale
            Bitmap grayImage1 = ConvertToGrayscale(img1);
            Bitmap grayImage2 = ConvertToGrayscale(img2);

            // Apply Laplacian filter
            Convolution convolutionFilter = new Convolution(new int[,]
            {
                { 0,  1, 0 },
                { 1, -4, 1 },
                { 0,  1, 0 }
            });

            // Ensure images have a compatible pixel format for the filter
            //if (grayImage1.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                grayImage1 = grayImage1.Clone(new Rectangle(0, 0, grayImage1.Width, grayImage1.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }

            //if (grayImage2.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                grayImage2 = grayImage2.Clone(new Rectangle(0, 0, grayImage2.Width, grayImage2.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }

            Bitmap image1 = convolutionFilter.Apply(grayImage1);
            Bitmap image2 = convolutionFilter.Apply(grayImage2);

            double nlseImg1 = CalculateNLSEForImage(image1);
            double nlseImg2 = CalculateNLSEForImage(image2);

            return (nlseImg1, nlseImg2);
        }

        private static Bitmap ConvertToGrayscale(Bitmap originalBitmap)
        {
            // Create a new grayscale bitmap
            Bitmap grayscaleBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format24bppRgb);

            // Loop through each pixel and convert to grayscale
            for (int x = 0; x < originalBitmap.Width; x++)
            {
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    Color originalColor = originalBitmap.GetPixel(x, y);

                    // Calculate grayscale value using the average of RGB values
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);

                    // Set the same grayscale value for all RGB channels
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

                    // Set the pixel in the new grayscale bitmap
                    grayscaleBitmap.SetPixel(x, y, grayColor);
                }
            }

            return grayscaleBitmap;
        }

        private static double CalculateNLSEForImage(Bitmap image)
        {
            double sumSquaredLaplacian = 0.0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    double value = pixel.R / 255.0; // Assuming grayscale, normalize to [0, 1]
                    sumSquaredLaplacian += value * value;
                }
            }

            return sumSquaredLaplacian / (image.Width * image.Height);
        }

        public static double CalculateNCC(Bitmap image1, Bitmap image2)
        {
            // Ensure images have the same size
            if (image1.Size != image2.Size)
                throw new ArgumentException("Images must have the same size.");

            // Calculate mean values
            float mean1 = CalculateMean(image1);
            float mean2 = CalculateMean(image2);

            // Calculate cross-correlation
            float crossCorrelation = 0;
            float sum1 = 0;
            float sum2 = 0;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    float value1 = image1.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]
                    float value2 = image2.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]

                    crossCorrelation += (value1 - mean1) * (value2 - mean2);
                    sum1 += (value1 - mean1) * (value1 - mean1);
                    sum2 += (value2 - mean2) * (value2 - mean2);
                }
            }

            // Calculate NCC
            float ncc = crossCorrelation / (float)Math.Sqrt(sum1 * sum2);
            return ncc;
        }

        public static double CalculateZNCC(Bitmap image1, Bitmap image2)
        {
            // Ensure images have the same size
            if (image1.Size != image2.Size)
                throw new ArgumentException("Images must have the same size.");

            // Calculate mean values
            float mean1 = CalculateMean(image1);
            float mean2 = CalculateMean(image2);

            // Calculate standard deviations
            float stdDev1 = CalculateStdDev(image1, mean1);
            float stdDev2 = CalculateStdDev(image2, mean2);

            // Calculate cross-correlation
            float crossCorrelation = 0;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    float value1 = image1.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]
                    float value2 = image2.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]

                    crossCorrelation += ((value1 - mean1) / stdDev1) * ((value2 - mean2) / stdDev2);
                }
            }

            // Calculate ZNCC
            float zncc = crossCorrelation / (image1.Width * image1.Height);
            return zncc;
        }

        public static float CalculateMean(Bitmap image)
        {
            float sum = 0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    sum += image.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]
                }
            }

            return sum / (image.Width * image.Height);
        }

        public static float CalculateStdDev(Bitmap image, float mean)
        {
            float sum = 0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float value = image.GetPixel(x, y).R / 255.0f; // Normalize to [0, 1]
                    sum += (value - mean) * (value - mean);
                }
            }

            return (float)Math.Sqrt(sum / (image.Width * image.Height));
        }
        #endregion
    }

    public enum FlipType
    {
        Horizontal,
        Vertical
    }
}
