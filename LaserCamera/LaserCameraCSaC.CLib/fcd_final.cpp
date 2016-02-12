
#include "ImgProcLib.h"

#include <stdlib.h>

#include <time.h>
#include <conio.h>

#define LAST_IMG_MIDDLE 9
#define LAST_IMG_COUNT 20


extern "C" {
	


	int** LastXImages = NULL;
	int replaceImage = 0;

	int* LastXCrossesX = NULL;
	int* LastXCrossesY = NULL;
	int lastCross = 0;

	int CenterX = 0;
	int CenterY = 0;

	uchar* Original = NULL;
	int* Image = NULL;
	int Width = 0;
	int Height = 0;
	int WHSize = 0;

	int dispImage = 0;

	int dispThresImg = 0;
	int dispFixedCross = 0;
	int dispDetectedCross = 0;

	int invertThreshold = 0;

	int thresholdValue = 128;

	int doProcessImage = 0;

	void WriteDataToOriginal(int* img){
		uchar* oPtr = Original;
		for (int i = 0; i < WHSize; ++i){			
			*oPtr = oPtr[1] = oPtr[2] = img[i];
			oPtr += 3;
		}		
	}

	void WriteOverlayers(int* thresimg, int cx, int cy){

		uchar* oPtr = Original;
		if (dispThresImg > 0){
			oPtr = Original;
			for (int i = 0; i < WHSize; ++i){
				if (thresimg[i] > 128){
					/*oPtr[0] = 237;//Blue
					oPtr[1] = 145;//Green
					oPtr[2] = 86;//Red*/
					oPtr[0] = 234;//Blue
					oPtr[1] = 124;//Green
					oPtr[2] = 53;//Red
				}
				oPtr += 3;
			}
		}
		if (dispFixedCross > 0){
			int x = Width / 2;
			int y = Height / 2;
			oPtr = &Original[y*Width * 3];
			for (int i = 0; i < Width; ++i){
				oPtr[0] = 0;//Blue
				oPtr[1] = 0;//Green
				oPtr[2] = 255;//Red
				oPtr += 3;
			}
			oPtr = Original;
			int p = 0;
			for (int i = 0; i < Height; ++i){
				p = (i*Width) * 3 + x * 3;
				oPtr[p] = 0;
				oPtr[p + 1] = 0;//Green
				oPtr[p + 2] = 255;//Red
			}
		}
		if (dispDetectedCross > 0){
			oPtr = &Original[cy*Width * 3];
			for (int i = 0; i < Width; ++i){
				oPtr[0] = 0;//Blue
				oPtr[1] = 255;//Green
				oPtr[2] = 0;//Red
				oPtr += 3;
			}
			oPtr = Original;
			int p = 0;
			for (int i = 0; i < Height; ++i){
				p = (i*Width) * 3 + cx * 3;
				oPtr[p] = 0;
				oPtr[p + 1] = 255;//Green
				oPtr[p + 2] = 0;//Red
			}
		}
	}

	__declspec(dllexport) void TestAndLoadLib(){		
		printf("Use C Lib for cross detection (fcd_final.cpp) !\n");
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height){
		Original = (uchar*)a;
		if (Width != width || Height != height){
			Width = width;
			Height = height;
			WHSize = width*height;
			if (Image != NULL)
				free(Image);
			Image = (int*)malloc(WHSize * sizeof(int));
			for (int i = 0; i < WHSize; ++i)
				Image[i] = 0;

			if (LastXImages != NULL){
				for (int i = 0; i < LAST_IMG_COUNT; ++i)
					free(LastXImages[i]);
				free(LastXImages);
			}
			LastXImages = dcalloc(LAST_IMG_COUNT, int*);
			for (int i = 0; i < LAST_IMG_COUNT; ++i){
				LastXImages[i] = dcalloc(WHSize, int);
			}
			replaceImage = 0;
			
			if (LastXCrossesX != NULL)
				free(LastXCrossesX);
			LastXCrossesX = dcalloc(LAST_IMG_COUNT, int);

			if (LastXCrossesY != NULL)
				free(LastXCrossesY);
			LastXCrossesY = dcalloc(LAST_IMG_COUNT, int);
			lastCross = 0;
			
			
		}
		uchar* oPtr = Original;		
		for (int i = 0; i < WHSize; ++i){
			Image[i] = 0;// (int)(LAST_IMGS_BIGGER_PART* (double)(Image[i])) + (LAST_IMAS_LOWER_PART * (double)(*oPtr));// *oPtr;
			LastXImages[replaceImage][i] = *oPtr;
			oPtr += 3;
		}
		++replaceImage;
		if (replaceImage >= LAST_IMG_COUNT)
			replaceImage = 0;
		//printf("%d\n",replaceImage);
	}


	__declspec(dllexport) void ProcessImage_old(){
		//Step 1: Adjust Image
		reduceNoise(Image, Width, Height, 5);
		imadjst(Image, WHSize, 0.01);
		if (dispImage == 1)
			WriteDataToOriginal(Image);

		//Step 2: Reduce Background
		reduceBackground(Image, Width, Height, 25);
		if (dispImage == 2)
			WriteDataToOriginal(Image);

		//Step 3: thresholdimage
		int* timg = thresholdImage(Image, Width, Height, thresholdValue);
		if (invertThreshold > 0){
			for (int i = 0; i < WHSize; ++i){
				timg[i] = timg[i] >= 128 ? 0 : 255;
			}
		}
		if (dispImage == 3)
			WriteDataToOriginal(timg);

		int cx = 0;
		int cy = 0;
		int* cimg = crossDetectionOfThreshold(timg, Width, Height, cx, cy);
		if (dispImage == 5){
			WriteDataToOriginal(cimg);
		}



		//edgeDetectionOfThreshold
		int* eimg = edgeDetectionOfThreshold(cimg, Width, Height, 128, 0);
		/*for (int i = 0; i < WHSize; ++i)
		eimgX[i] = eimgX[i] == 255 && eimgY[i] == 255 ? 255 : 0;*/
		if (dispImage == 4)
			WriteDataToOriginal(eimg);


		//WriteDataToOriginal(cimg);
		crossCenterDetection_CoM(eimg, Width, Height, 128, 0, Width, 0, Height, cx, cy);

		for (int i = 0; i <= 4; ++i){
			int x = cx - Width / 4;
			int y = cy - Height / 4;
			crossCenterDetection(eimg, Width, Height, 128, x, cx + (Width / 4), y, cy + (Height / 4), cx, cy);
		}

		WriteOverlayers(timg, cx, cy);

		CenterX = cx;
		CenterY = cy;

		free(timg);
		free(eimg);
		free(cimg);
	}

	__declspec(dllexport) void ProcessImage(){	
		
		if (doProcessImage > 0){
			int* img;
			for (int i = 0; i < LAST_IMG_COUNT; ++i){
				img = LastXImages[i];
				for (int xy = 0; xy < WHSize; ++xy){
					Image[xy] += img[xy];// LastXImages[i][xy];
				}
			}

			for (int xy = 0; xy < WHSize; ++xy){
				Image[xy] = Image[xy] / LAST_IMG_COUNT;// (int)(((double)Image[xy]) / ((double)LAST_IMG_COUNT));
			}


			reduceNoise(Image, Width, Height, 1);


			//Step 1: Adjust Image (~20ms)
			imadjst(Image, WHSize, 0.01);
			if (dispImage == 1)
				WriteDataToOriginal(Image);


			//Step 2: Reduce Background
			reduceBackground(Image, Width, Height, 25);
			if (dispImage == 2)
				WriteDataToOriginal(Image);


			//Step 3: thresholdimage (~70ms)
			int* timg = thresholdImage(Image, Width, Height, thresholdValue);
			if (invertThreshold > 0){
				for (int i = 0; i < WHSize; ++i){
					timg[i] = timg[i] >= 128 ? 0 : 255;
				}
			}
			if (dispImage == 3)
				WriteDataToOriginal(timg);

			int* eimg = edgeDetection(timg, Width, Height, 128, 0);
			if (dispImage == 4)
				WriteDataToOriginal(eimg);

			int cx = 0;
			int cy = 0;
			crossCenterDetection(eimg, Width, Height, 128, 0, Width, 0, Height, cx, cy);

			for (int i = 0; i <= 4; ++i){
				int x = cx - Width / 8;
				int y = cy - Height / 8;
				crossCenterDetection(eimg, Width, Height, 128, x, cx + (Width / 8), y, cy + (Height / 8), cx, cy);
			}
			LastXCrossesX[lastCross] = cx;
			LastXCrossesY[lastCross] = cy;
			++lastCross;
			if (lastCross >= LAST_IMG_COUNT)
				lastCross = 0;

			

			CenterX = 0;
			CenterY = 0;
			for (int i = 0; i < LAST_IMG_COUNT; ++i){
				CenterX += LastXCrossesX[i];
				CenterY += LastXCrossesY[i];
			}
			CenterX /= LAST_IMG_COUNT;
			CenterY /= LAST_IMG_COUNT;
			WriteOverlayers(timg, CenterX, CenterY);
			free(eimg);
			free(timg);
		}
		else{
			int* zeros = dcalloc(WHSize, int);
			WriteOverlayers(zeros, CenterX, CenterY);
			free(zeros);
		}

		
		
	}


	__declspec(dllexport) void SetDispImage(int outimg){
		dispImage = outimg % 6;
	}

	__declspec(dllexport) void SetOverlayer(int layer, int value){
		if (layer == 0)
			dispThresImg = value;
		if (layer == 1)
			dispFixedCross = value;
		if (layer == 2)
			dispDetectedCross = value;

		if (layer == 3)
			invertThreshold = value;

		if (layer == 4)
			thresholdValue = value;
	}

	__declspec(dllexport) void SetParameter(int outputImage, int outputCross){

	}

	__declspec(dllexport) bool GetCrossFound(){
		return true;
	}

	__declspec(dllexport) void SetDoProcess(int val){
		doProcessImage = val;
	}

	__declspec(dllexport) void SetBWThreshold(double val){
		//BWThreshold = val;
	}

	__declspec(dllexport) double X_CenterDistancePercet(){
		double w2 = ((double)Width) / 2.0;
		double cx = (double)CenterX;
		return ((cx - w2) / w2) * 100.0;
	}

	__declspec(dllexport) double Y_CenterDistancePercet(){
		double h2 = ((double)Height) / 2.0;
		double cy = (double)CenterY;
		return ((cy - h2) / h2) * 100.0;
	}
}