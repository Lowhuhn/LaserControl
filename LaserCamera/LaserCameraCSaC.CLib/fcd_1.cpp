#include <ios>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define uchar unsigned char 
#define NUM double


extern "C" {
	/*//0 = Eingabe, 1 = LightCorrection, 2 = Normalized, 3 = BlackWhite, 4 = NoiseReduction, 5 Inverted
	int DrawingOutputImage = 0;
	//0 = kein Kreuz, 1 = Mittelkreuz, 2 = Gefundenes Kreuz, 3 = Beide Kreuze
	int DrawingCross = 0;

	int Width = 0;
	int Height = 0;
	int WidthHeight = 0;
	int WidthM1 = 0;
	int HeightM1 = 0;

	int CenterX = 0,
		CenterY = 0;

	bool CrossFound = false;

	uchar* Image;
	int* UseImage;

	int Counter = 0;

	double BWThreshold = 0.25;

	void Step1(){
		int x = 0, y = 0, a = 0, posX = 0, posY = 0;
		int min = INT_MAX, max = INT_MIN;
		for (x = 0; x < WidthHeight; ++x)
		{
			if (UseImage[x] < min)
				min = UseImage[x];
			if (UseImage[x] > max)
				max = UseImage[x];
		}
		max = max - min;
		for (x = 0; x < WidthHeight; ++x)
		{
			UseImage[x] = (((((double)UseImage[x]) - min) / max) >= BWThreshold ? 255 : 0);
		}

		for (y = 1; y < HeightM1; ++y){
			posY = y*Width;
			for (x = 1; x < WidthM1; ++x){
				posX = posY + x;
				a = (UseImage[posX - Width - 1] + UseImage[posX - Width] + UseImage[posX - Width + 1] +
					UseImage[posX - 1] + UseImage[posX + 1] +
					UseImage[posX + Width - 1] + UseImage[posX + Width] + UseImage[posX + Width + 1])/255;
				if (UseImage[posX] == 0){					
					if (a >= 5)
						UseImage[posX] = 255;
				}
				else{
					if (a <= 2){
						UseImage[posX] = 0;
					}
				}
			}
		}
	}

	void Step2(){
		int x = 0, y = 0, posX = 0, posY = 0;
		int min = INT_MAX, max = INT_MIN;
		int* use = (int*)malloc(WidthHeight*sizeof(int));
		double d = 0;
		for (x = 0; x < WidthHeight; ++x)
			use[x] = 0;

		for (y = 1; y < HeightM1; ++y){
			posY = y*Width;
			for (x = 1; x < WidthM1; ++x){
				posX = posY + x;				
					use[posX] = (UseImage[posX - Width - 1] + 2*UseImage[posX - Width] + UseImage[posX - Width + 1] +
						2 * UseImage[posX - 1] + 4*UseImage[posX]+ 2*UseImage[posX + 1] +
						UseImage[posX + Width - 1] + 2*UseImage[posX + Width] + UseImage[posX + Width + 1]) / 16;					
				//}
			}
		}
		
		free(use);
	}

	void Step3(){
		int x = 0, y = 0, posX = 0, posY = 0, sx = 0, sy = 0;
		int min = INT_MAX, max = INT_MIN, a = 0;
		int* use = (int*)malloc(WidthHeight*sizeof(int));
		for (x = 0; x < WidthHeight; ++x)
			use[x] = 0;
		
		for (y = 1; y < HeightM1; ++y){
			posY = y*Width;
			for (x = 1; x < WidthM1; ++x){
				posX = posY + x;
				sx = UseImage[posX - Width - 1] - UseImage[posX - Width + 1] +
						2 * UseImage[posX - 1] - 2 * UseImage[posX + 1] +
						UseImage[posX + Width - 1] - UseImage[posX + Width + 1];
				sy = UseImage[posX - Width - 1] + 2 * UseImage[posX - Width] + UseImage[posX - Width + 1] -
					(UseImage[posX + Width - 1] + 2 * UseImage[posX + Width] + UseImage[posX + Width + 1]);
				use[posX] = (abs(sx) > 0 || abs(sy) > 0) ? 255 : 0;// sqrt(sx*sx + sy*sy) > 0 ? 255 : 0;

			}
		}
		for (x = 0; x < WidthHeight; ++x)
			UseImage[x] = use[x];

		free(use);		
	}

	void Step3_2(){
		int x = 0, y = 0, posX = 0, posY = 0, a = 0;
		for (y = 1; y < HeightM1; ++y){
			posY = y*Width;
			for (x = 1; x < WidthM1; ++x){
				posX = posY + x;
				a = (UseImage[posX - Width - 1] + UseImage[posX - Width] + UseImage[posX - Width + 1] +
					UseImage[posX - 1] + UseImage[posX + 1] +
					UseImage[posX + Width - 1] + UseImage[posX + Width] + UseImage[posX + Width + 1]) / 255;
				if (UseImage[posX] == 255){
					if (a >= 3)
						UseImage[posX] = 0;
				}
			}
		}
	}

	void Step4(){
		int x = 0, y = 0, posY = 0, posX = 0, a = 0;
		int ax = 0, ay = 0, n = 0;
		for (y = 0; y < Height; ++y){
			posY = y*Width;
			for (x = 0; x < Width; ++x){
				posX = posY + x;
				if (UseImage[posX] > 0){
					a = (UseImage[posX - Width - 1] + UseImage[posX - Width] + UseImage[posX - Width + 1] +
						UseImage[posX - 1] + UseImage[posX + 1] +
						UseImage[posX + Width - 1] + UseImage[posX + Width] + UseImage[posX + Width + 1]) / 255;
					ax += x;
					ay += y;
					n += 1;
				}
			}
		}
		if (n > 0){
			CenterX = ax / n;
			CenterY = ay / n;
			CrossFound = true;
		}
		else{
			CrossFound = false;
		}
	}

	void Step4_2(int sizeHalf){
		int x = 0, y = 0, posY = 0, posX = 0;
		int a = 0;
		int ax = 0, ay = 0, nx = 0, ny = 0, wx = 0, wy = 0;

		sizeHalf *= 2;

		int widthHalf = (Width / sizeHalf);
		int heightHalf = (Height / sizeHalf);

		int sx = CenterX - widthHalf;
		if (sx < 0)
			sx = 0;
		int sy = CenterY - heightHalf;
		if (sy < 0)
			sy = 0;

		int ex = CenterX + widthHalf;
		if (ex > Width){
			ex = Width;
			sx = Width - widthHalf;
		}
		int ey = CenterY + heightHalf;
		if (ey > Height){
			ey = Height;
			sy = Height - heightHalf;
		}



		for (y = sy; y < ey; ++y){
			posY = y*Width;
			for (x = sx; x < ex; ++x){
				posX = posY + x;
				if (UseImage[posX] > 0){
					wx = (widthHalf - abs(x - CenterX));
					wy = (heightHalf - abs(y - CenterY));
					ax += wx*x;
					ay += wy*y;
					nx += wx;
					ny += wy;
				}
			}
		}
		if (nx > 0 && ny > 0){
			CenterX = (int)(ax / nx);
			CenterY = (int)(ay / ny);
		}
	}

	void Step4_3(int sizeHalf){
		int x = 0, y = 0, posY = 0, posX = 0;
		int a = 0;
		int ax = 0, ay = 0, n = 0;

		sizeHalf *= 2;

		int widthHalf = (Width / sizeHalf);
		int heightHalf = (Height / sizeHalf);

		int sx = CenterX - widthHalf;
		if (sx < 0)
			sx = 0;
		int sy = CenterY - heightHalf;
		if (sy < 0)
			sy = 0;

		int ex = sx + widthHalf;
		if (ex > Width){
			ex = Width;
			sx = Width - widthHalf;
		}
		int ey = sy + heightHalf;
		if (ey > Height){
			ey = Height;
			sy = Height - heightHalf;
		}



		for (y = sy; y < ey; ++y){
			posY = y*Width;
			for (x = sx; x < ex; ++x){
				posX = posY + x;
				if (UseImage[posX] > 0){
					ax += x;
					ay += y;
					n++;
				}
			}
		}
		if (n > 0 ){
			CenterX = (ax / n);
			CenterY = (ay / n);
		}
	}

	void ImageProcessing_CrossCenterDetection(int *img){
		int x = 0, y = 0, py = 0;

		NUM a = 0, bc = 0, d = 0, e = 0, f = 0, m = 0, n = 0;
		NUM bcy = 0, dy = 0, ey = 0, fy = 0, my = 0, ny = 0;
		NUM cx = 0;

		for (y = 0; y < Height; ++y){
			py = y*Width;
			for (x = 0; x < Width; ++x){
				if (img[py + x] > 128){
					++a;

					//X
					bc += x;
					d += (x*x);
					e += y;
					f += (x*y);

					//Y
					dy += (y*y);
				}
			}
		}
		//Set values that are equal
		bcy = e;
		ey = bc;
		fy = f;

		f -= ((bc / a)*e);
		d -= ((bc / a)*bc);
		n = f / d;
		bc *= n;
		e -= bc;
		m = e / a;

		fy -= ((bcy / a)*ey);
		dy -= ((bcy / a)*bcy);
		ny = fy / dy;
		bcy *= ny;
		ey -= bcy;
		my = ey / a;


		cx = ((m*ny) + my) / (1 - n*ny);

		CenterX = (int)cx;
		CenterY = (int)(m + (n*cx));

		CrossFound = (0 <= CenterX && CenterX < Width && 0 <= CenterY && CenterY < Height);

		if (CenterX < 0)
			CenterX = 0;
		if (CenterX >= Width)
			CenterX = 0;
		if (CenterY < 0)
			CenterY = 0;
		if (CenterY >= Height)
			CenterY = 0;
	}

	void WriteDataBack(){
		uchar* img = Image;
		for (int i = 0; i < WidthHeight; ++i){
			*(img) = img[1] = img[2] = (uchar)UseImage[i];
			img += 3;
		}
	}


	void ImageProcessing_Normalization(int *img){
		int* vals = (int*)malloc(256 * sizeof(int));
		int* cfd = (int*)malloc(256 * sizeof(int));
		int* h = (int*)malloc(256 * sizeof(int));

		for (int i = 0; i < 256; ++i){
			vals[i] = 0;
		}

		for (int i = 0; i < WidthHeight; ++i){
			vals[img[i]]++;
		}

		int minCFD = 0;
		for (int i = 0; i < 256; ++i){
			minCFD += vals[i];
			cfd[i] = minCFD;
		}

		int j = 0;
		while (vals[j] == 0 && j < 255)
			j++;

		if (j > 255)
			j = 255;

		minCFD = cfd[j];

		for (int i = 0; i < 256; ++i){
			if (vals[i] > 0){
				h[i] = lround((((double)(cfd[i] - minCFD)) / ((double)(WidthHeight - minCFD)))*255.0);
			}
		}
		for (int i = 0; i < WidthHeight; ++i){
			img[i] = h[img[i]];
		}

		free(vals);
		free(cfd);
		free(h);
	}

	void ImageProcessing_LightCorrection(int *img){
		int a = img[0];
		int b = img[Width - 1];
		int c = img[WidthHeight - Width + 1];
		int d = img[WidthHeight - 1];


		int m = (a + b + c + d) / 4;
		//printf("a = %d\nb = %d\nc = %d\nd = %d\n", a, b, c, d);

		int ac = a > c ? -1 : 1;
		int bd = b > d ? -1 : 1;
		int lr = a > b ? -1 : 1;

		int py = 0, x = 0, y = 0;

		int colWidth = 0;

		int colHeightLeft = ceil(Height / (double)(abs(a - c) + 1));
		int colHeightRight = ceil(Height / (double)(abs(b - d) + 1));

		int lColor = 0, rColor = 0;

		//printf("a = %d\nb = %d\nc = %d\nd = %d\n", a, b, c, d);
		//printf("colHeightLeft: %d -- colHeightRight: %d\n", colHeightLeft, colHeightRight);

		NUM v = 0;

		for (y = 0; y < Height; ++y){
			py = y*Width;
			lColor = a + (ac)*(y / colHeightLeft);
			rColor = b + (bd)*(y / colHeightRight);

			colWidth = ceil(Width / (double)(abs(lColor - rColor) + 1));
			for (x = 0; x < Width; ++x){
				v = lColor + (lr)*(x / colWidth);
				if (v > 0)
				{
					img[py + x] = (int)(m * (img[py + x] / v));
				}
			}
		}

		for (x = 0; x < WidthHeight; ++x){
			if (img[x] > 255){
				img[x] = 255;
			}
			if (img[x] < 0){
				img[x] = 0;
			}
		}
	}

	__declspec(dllexport) void TestAndLoadLib(){
		printf("Use C Lib for cross detection!\n");
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height){
		Image = (uchar*)a;
		Width = width;
		Height = height;
		WidthM1 = Width - 1;
		HeightM1 = Height - 1;
		if (Width*Height != WidthHeight){
			free(UseImage);
			WidthHeight = Width*Height;
			UseImage = (int*)malloc(WidthHeight * sizeof(int));	
		}		
		uchar* img = Image;
		for (int x = 0; x < WidthHeight; ++x){
			UseImage[x] = *img;
			img += 3;
		}
	}

	__declspec(dllexport) void Find(){		

		//Step1();
		ImageProcessing_Normalization(UseImage);
		ImageProcessing_LightCorrection(UseImage);
		Step1();
		if (DrawingOutputImage == 1)
			WriteDataBack();

		Step2();
		if (DrawingOutputImage == 2)
			WriteDataBack();

		Step3();
		if (DrawingOutputImage == 3)
			WriteDataBack();

		Step3_2();
		if (DrawingOutputImage == 4)
			WriteDataBack();

		Step4();
		if (CrossFound){
			for (int i = 0; i < 25; ++i)
				Step4_2(2);
			for (int i = 0; i < 25; ++i)
				Step4_2(3);
		}

		if (CenterX < 0)
			CenterX = 0;
		if (CenterX >= Width)
			CenterX = WidthM1;
		if (CenterY < 0)
			CenterY = 0;
		if (CenterY >= Height)
			CenterX = HeightM1;
		Counter = 0;
		

		uchar* img = Image;
		if (DrawingCross == 1 || DrawingCross == 3){
			int cx = Width / 2;
			int cy = Height / 2;
			img = &Image[cy*Width * 3];
			for (int i = 0; i < Width; ++i){
				img[0] = 0;
				img[1] = 0;//Green
				img[2] = 255;//Red
				img += 3;
			}
			img = Image;
			int p = 0;
			for (int i = 0; i < Height; ++i){
				p = (i*Width) * 3 + cx * 3;
				img[p] = 0;
				img[p + 1] = 0;//Green
				img[p + 2] = 255;//Red
			}
		}

		if (DrawingCross == 2 || DrawingCross == 3){

			img = &Image[CenterY*Width * 3];
			for (int i = 0; i < Width; ++i){
				img[0] = 0;
				img[1] = 255;//Green
				img[2] = 0;//Red
				img += 3;
			}
			img = Image;
			int p = 0;
			for (int i = 0; i < Height; ++i){
				p = (i*Width) * 3 + CenterX * 3;
				img[p] = 0;
				img[p + 1] = 255;//Green
				img[p + 2] = 0;//Red
			}
		}
		++Counter;
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
		return CrossFound;
	}

	__declspec(dllexport) void SetBWThreshold(double val){
		BWThreshold = val;
	}*/
}