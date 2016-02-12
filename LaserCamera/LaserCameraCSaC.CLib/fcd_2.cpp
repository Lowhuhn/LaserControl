#include <ios>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define uchar unsigned char 
#define NUM double
#define BUFFERSIZE 10

extern "C" {
	/*//0 = Eingabe, 1 = LightCorrection, 2 = Normalized, 3 = BlackWhite, 4 = NoiseReduction, 5 Inverted
	int DrawingOutputImage = 0;
	//0 = kein Kreuz, 1 = Mittelkreuz, 2 = Gefundenes Kreuz, 3 = Beide Kreuze
	int DrawingCross = 0;

	bool Alloced = false;
	int Counter = 0; 
	int Height = 0;
	int Width = 0;
	int WidthHeight = 0;
	
	int** Images = (int**)malloc(BUFFERSIZE * sizeof(int*));
	uchar* Image = NULL;

	void Step3(int* img){
		int x = 0, y = 0, posX = 0, posY = 0, sx = 0, sy = 0;
		int min = INT_MAX, max = INT_MIN, a = 0;
		int* use = (int*)malloc(WidthHeight*sizeof(int));
		for (x = 0; x < WidthHeight; ++x)
			use[x] = 0;

		for (y = 1; y < Height-1; ++y){
			posY = y*Width;
			for (x = 1; x < Width-1; ++x){
				posX = posY + x;
				sx = img[posX - Width - 1] - img[posX - Width + 1] +
					2 * img[posX - 1] - 2 * img[posX + 1] +
					img[posX + Width - 1] - img[posX + Width + 1];
				sy = img[posX - Width - 1] + 2 * img[posX - Width] + img[posX - Width + 1] -
					(img[posX + Width - 1] + 2 * img[posX + Width] + img[posX + Width + 1]);
				use[posX] = (abs(sx) > 0 || abs(sy) > 0) ? 255 : 0;// sqrt(sx*sx + sy*sy) > 0 ? 255 : 0;

			}
		}
		for (x = 0; x < WidthHeight; ++x)
			img[x] = use[x];

		free(use);
	}

	void WriteDataBack(int* use){
		uchar* img = Image;
		for (int i = 0; i < WidthHeight; ++i){
			*(img) = img[1] = img[2] = (uchar)use[i];
			img += 3;
		}
	}

	__declspec(dllexport) void TestAndLoadLib(){
		printf("Use C Lib for cross detection (fcd_2.cpp) !\n");
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height){
		if (Width != width || Height != height){
			if (Alloced){
				for (int i = 0; i < BUFFERSIZE; ++i){
					free(Images[i]);
				}
			}
			Width = width;
			Height = height;
			WidthHeight = Width*Height;
			for (int i = 0; i < BUFFERSIZE; ++i){
				Images[i] = (int*)malloc(WidthHeight*sizeof(int));
			}
			Counter = 0;
			Alloced = true;
		}

		uchar* img = (uchar*)a;
		for (int i = 0; i < WidthHeight; ++i){
			Images[Counter][i] = (int)*img;
			img += 3;
		}
		Counter++;
		if (Counter >= BUFFERSIZE)
			Counter = 0;

		Image = (uchar*)a;
	}

	__declspec(dllexport) void Find(){
		printf("Hallo Welt %d\n", WidthHeight);
		if (Counter == (BUFFERSIZE - 1)){
			int* img = Images[Counter];			
			Step3(img);
			WriteDataBack(img);
		}
	}

	__declspec(dllexport) void SetParameter(int outputImage, int outputCross){
		if (outputImage < 0)
			outputImage *= (-1);
		if (outputCross < 0)
			outputCross *= (-1);

		DrawingOutputImage = outputImage;
		DrawingCross = outputCross;
	}

	__declspec(dllexport) bool GetCrossFound(){
		return true;;
	}

	__declspec(dllexport) void SetBWThreshold(double val){
		//BWThreshold = val;
	}*/
}