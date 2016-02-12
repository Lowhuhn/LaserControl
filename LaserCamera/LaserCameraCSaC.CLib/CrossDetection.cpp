
#include "definitions.h"

extern "C" {
	/*
	bool CrossFound = true;

	int Width = 0;
	int Height = 0;
	int WidthHeight = 0;

	uchar* Image;

	//0 = Eingabe, 1 = LightCorrection, 2 = Normalized, 3 = BlackWhite, 4 = NoiseReduction, 5 Inverted
	int DrawingOutputImage = 0;
	//0 = kein Kreuz, 1 = Mittelkreuz, 2 = Gefundenes Kreuz, 3 = Beide Kreuze
	int DrawingCross = 3;

	int CenterX = 0;
	int CenterY = 0;

	void ImageProcessing_InvertIfNecessary2(int* img){
		int med = 0;
		int i = 0;

		for (i = 0; i < WidthHeight; ++i){
			med += img[i];
		}
		med /= WidthHeight;
		//printf("med: %d\n", med);
		if (med > 128){
			for (i = 0; i < WidthHeight; ++i)
				img[i] = 255 - img[i];

		}
	}

	int cmpfunc(const void * a, const void * b)
	{
		return (*(int*)a - *(int*)b);
	}

	void ImageProcessing_BWImg2(int* img){		
		for (int i = 0; i < WidthHeight; ++i){
			img[i] = img[i] < 35 ? 0 : 255;
		}
	}

	void ImageProcessing_NoiseReduction2(int *img){
		int x = 0, y = 0, py = 0;
		int m = 0;
		for (y = 1; y < Height - 1; ++y)
		{
			py = y*Width;
			for (x = 1; x < Width - 1; ++x){
				m = img[py + x - 1] + img[py + x + 1] + img[py + x - Width] + img[py + x + Width];				
				m /= 4;
				img[py + x] = img[py + x] < m ? m : img[py + x];
			}
		}
		ImageProcessing_BWImg2(img);
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
		int c = img[WidthHeight-Width + 1];
		int d = img[WidthHeight - 1];


		int m = (a + b + c + d) / 4;
		//printf("a = %d\nb = %d\nc = %d\nd = %d\n", a, b, c, d);

		int ac = a > c ? -1 : 1;
		int bd = b > d ? -1 : 1;
		int lr = a > b ? -1 : 1;

		int py = 0,x = 0,y=0;

		int colWidth = 0;

		int colHeightLeft = ceil(Height / (double)(abs(a - c)+1));
		int colHeightRight = ceil(Height / (double)(abs(b - d)+1));
		
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

	void WriteDataBack(int *use){
		uchar* img = Image;
		for (int i = 0; i < WidthHeight; ++i){
			*(img) = img[1] = img[2] = (uchar)use[i];
			img += 3;
		}
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height)
	{
		Image = (unsigned char*)a;
		Width = width;
		Height = height;
		WidthHeight = Width*Height;
	}

	__declspec(dllexport) void Find(){
		//Var declarations
		int i = 0, x = 0;		
		int* use = (int*)malloc(WidthHeight*sizeof(int));
		
		uchar* img = Image;

		for (x = 0; x < WidthHeight; ++x){
			use[x] = *img;
			img += 3;
		}

		ImageProcessing_LightCorrection(use);	
		if (DrawingOutputImage == 1)
			WriteDataBack(use);
		ImageProcessing_Normalization(use);		
		if (DrawingOutputImage == 2)
			WriteDataBack(use);
		ImageProcessing_BWImg2(use);
		if (DrawingOutputImage == 3)
			WriteDataBack(use);
		ImageProcessing_NoiseReduction2(use);
		if (DrawingOutputImage == 4)
			WriteDataBack(use);
		ImageProcessing_InvertIfNecessary2(use);
		if (DrawingOutputImage == 5)
			WriteDataBack(use);

		ImageProcessing_CrossCenterDetection(use);

		//2. RUN :		
		for (int i = 2; i <= 8; ++i){
			int w2 = Width / i;
			int w4 = (Width / i) / 2;
			int nx0 = CenterX - w4;
			nx0 = nx0 < 0 ? 0 : nx0; // Kleiner 0?
			nx0 = (nx0 + w2) >= Width ? Width - 1 - w2 : nx0; // Zu nah an width rand
			int nx1 = nx0 + w2;

			int h2 = Height / i;
			int h4 = (Height / i) / 2;
			int ny0 = CenterY - h4;
			ny0 = ny0 < 0 ? 0 : ny0;
			ny0 = (ny0 + h2) >= Height ? Height - 1 - h2 : ny0;
			int ny1 = ny0 + h2;

			int py = 0, x = 0;
			for (int y = 0; y < Height; ++y){
				py = y*Width;
				for (x = 0; x < Width; ++x){
					if (x < nx0 || x > nx1 || y < ny0 || y > ny1)
						use[py + x] = 0;
				}
			}
			ImageProcessing_CrossCenterDetection(use);
		}

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
		free(use);
	}

	__declspec(dllexport) void TestAndLoadLib(){
		printf("Use C Lib for cross detection!\n");
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
	*/
}