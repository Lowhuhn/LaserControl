#include <ios>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <queue>

#define uchar unsigned char

struct MyPoint{
	int X;
	int Y;
};
struct MyPoint2{
	int X;
	int Y;
	double Val;
	//MyPoint2(int x, int y, NUM val) : X(x), Y(y), Val(val) {}

	bool operator < (const MyPoint2& other) const{
		return Val < other.Val;
	}
};

uchar* OrgImage = NULL;
int* Image = NULL;
int Width = 0;
int Height = 0;
int WHSize = 0;

void MarkAllObjects()
{
	int* J = (int*)malloc(WHSize*sizeof(int));
	for (int i = 0; i < WHSize; ++i){
		J[i] = 0;
	}

	std::queue<MyPoint> que;
	int xMin = 0, xMax = 0, yMin = 0, yMax = 0, size = 0;
	for (int y = 0; y < Height; ++y){
		for (int x = 0; x < Width; ++x){
			if (J[x + y*Width]>0)
				continue;
			if (Image[x + y*Width] == 0)
			{
				que.push({ x, y });
				xMin = Width;
				yMin = Height;
				xMax = 0;
				yMax = 0;
				size = 0;
				double a = 0, bc = 0, d = 0, e = 0, f = 0, m = 0, n = 0;
				double bcy = 0, dy = 0, ey = 0, fy = 0, my = 0, ny = 0;
				double cx = 0;
				while (!que.empty())
				{
					MyPoint p = que.front();
					que.pop();
					if (J[p.X + p.Y*Width] > 0 || Image[p.X+p.Y*Width] > 0)
						continue;

					++size;
					J[p.X + p.Y*Width] = 1;
					
					++a;

					//X
					bc += p.X;
					d += (p.X*p.X);
					e += p.Y;
					f += (p.X*p.Y);

					//Y
					dy += (p.Y*p.Y);

					if (p.X < xMin)
						xMin = p.X;
					if (p.Y < yMin)
						yMin = p.Y;
					if (p.X > xMax)
						xMax = p.X;
					if (p.Y > yMax)
						yMax = p.Y;

					if (p.X > 0)
						que.push({ p.X - 1, p.Y });
					if (p.X < Width - 1)
						que.push({ p.X + 1, p.Y });
					if (p.Y > 0)
						que.push({ p.X, p.Y - 1 });
					if (p.Y < Height - 1)
						que.push({ p.X, p.Y + 1 });
				}
				//printf("ja: %d\t%d\t%d\t%d\n", xMin, xMax, yMin, yMax);
				if (size > 0.001*WHSize){

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
					int CenterX = (int)cx;
					int CenterY = (int)(m + (n*cx));
					if (CenterX < xMin)
						CenterX = xMin;
					if (CenterX >= xMax)
						CenterX = xMax-1;
					if (CenterY < yMin)
						CenterY = yMin;
					if (CenterY >= yMax)
						CenterY = yMax-1;

					for (int my = yMin; my < yMax; ++my){
						for (int mx = xMin; mx < xMax; ++mx){
							if (J[mx + my*Width] > 0){
								OrgImage[(mx + my*Width) * 3] = OrgImage[(mx + my*Width) * 3] = (uchar)255;
								OrgImage[(mx + my*Width) * 3 + 1] = OrgImage[(mx + my*Width) * 3 + 1] = (uchar)255;
								OrgImage[(mx + my*Width) * 3 + 2] = OrgImage[(mx + my*Width) * 3 + 2] = (uchar)0;
							}
						}
					}
					for (int my = yMin; my < yMax; ++my){
						OrgImage[(CenterX + my*Width) * 3] = (uchar)0;
						OrgImage[(CenterX + my*Width) * 3 + 1] = (uchar)255;
						OrgImage[(CenterX + my*Width) * 3 + 2] = (uchar)0;

						OrgImage[(xMin + my*Width) * 3] = OrgImage[(xMax + my*Width) * 3] = (uchar)0;
						OrgImage[(xMin + my*Width) * 3 + 1] = OrgImage[(xMax + my*Width) * 3 + 1] = (uchar)0;
						OrgImage[(xMin + my*Width) * 3 + 2] = OrgImage[(xMax + my*Width) * 3 + 2] = (uchar)255;
					}
					for (int mx = xMin; mx < xMax; ++mx){
						OrgImage[(mx + CenterY*Width) * 3] = (uchar)0;
						OrgImage[(mx + CenterY*Width) * 3 + 1] = (uchar)255;
						OrgImage[(mx + CenterY*Width) * 3 + 2] = (uchar)0;

						OrgImage[(mx + yMin*Width) * 3] = OrgImage[(mx + yMax*Width) * 3] = (uchar)0;
						OrgImage[(mx + yMin*Width) * 3 + 1] = OrgImage[(mx + yMax*Width) * 3 + 1] = (uchar)0;
						OrgImage[(mx + yMin*Width) * 3 + 2] = OrgImage[(mx + yMax*Width) * 3 + 2] = (uchar)255;
					}
				}
			}
		}
	}
	free(J);

}

extern "C"{
	
	/*
	int* ReginGrowing(int x, int y, double reg_maxdist = 0.02){
		double* I = (double*)malloc(WHSize*sizeof(double));
		int* J = (int*)malloc(WHSize*sizeof(int));
		for (int i = 0; i < WHSize; ++i){
			I[i] = ((double)Image[i]) / 255.0;
			J[i] = 0;
		}

		double reg_mean = I[x + y*Width];
		double reg_size = 1;

		MyPoint2 *neg_list = (MyPoint2*)malloc((WHSize+1)*sizeof(MyPoint2));

		int neg_list_size = 0;


		double pixdist = 0;

		MyPoint neigb[4] = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

		int i = 0;
		int xn = 0;
		int yn = 0;
		int index = 0;
		
		while (pixdist < reg_maxdist && reg_size < WHSize) {
			
			for (i = 0; i < 4; ++i){

				xn = x + neigb[i].X;
				yn = y + neigb[i].Y;

				if (((xn >= 0) && (xn < Width) && (yn >= 0) && (yn < Height)) && J[xn + yn*Width] == 0){
					neg_list[neg_list_size] = { xn, yn, I[xn + yn*Width] };
					++neg_list_size;

					J[xn + yn*Width] = 1;
				}
			}

			double lowBound = 0;
			int lowIndex = 0;
			double highBound = 2;
			int highIndex = 0;
			double v = 0;
			for (i = 0; i < neg_list_size; ++i){
				v = neg_list[i].Val;
				if (v > lowBound && v < highBound){
					if (v < reg_mean){
						lowBound = v;
						lowIndex = i;
					}
					else{
						highBound = v;
						highIndex = i;
					}
				}
			}
			lowBound = abs(lowBound - reg_mean);
			highBound = abs(highBound - reg_mean);
			if (lowBound < highBound){
				pixdist = lowBound;
				index = lowIndex;
			}
			else{
				pixdist = highBound;
				index = highIndex;
			}

			J[x + y*Width] = 2;
			++reg_size;

			reg_mean = (reg_mean*reg_size + neg_list[index].Val) / ((double)(reg_size + 1.0));

			x = neg_list[index].X;
			y = neg_list[index].Y;
			if (neg_list_size > 0)
				neg_list[index] = neg_list[neg_list_size - 1];
			--neg_list_size;
		}
		free(I);
		for (i = 0; i < WHSize; ++i){
			J[i] = J[i] > 1 ? 1 : 0;
		}
		return J;
	}

	__declspec(dllexport) void TestAndLoadLib(){
		printf("Use C Lib for cross detection (fcd_3.cpp) !\n");
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height){
		OrgImage = (uchar*)a;
		Width = width;
		Height = height;
		WHSize = Width* Height;
		uchar* ptr = (uchar*)a;
		if (Image != NULL)
			free(Image);
		Image = (int*)malloc(Width*Height*sizeof(int));
		int* ptr2 = Image;
		for (int y = 0; y < height; ++y){
			for (int x = 0; x < Width; ++x) {
				*ptr2 = (int)*ptr;
				++ptr2;
				ptr += 3;
			}
		}
	}

	

	void WriteDataBack(int* use){
		uchar* img = OrgImage;
		for (int i = 0; i < WHSize; ++i){
			*(img) = img[1] = img[2] = (uchar)use[i];
			img += 3;
		}
	}

	__declspec(dllexport) void Find(){
		//135 114
		//Search Cross width region growing
		double search = 0.02;
		int* out_1 = ReginGrowing(0, 0, search);
		int* out_2 = ReginGrowing(Width - 1, 0, search);
		int* out_3 = ReginGrowing(0, Height - 1, search);
		int* out_4 = ReginGrowing(Width - 1, Height - 1, search);

		for (int i = 0; i < WHSize; ++i){
			Image[i] = ((out_1[i] || out_2[i] || out_3[i] || out_4[i]) > 0 ? 255 : 0);
		}

		//MarkAllObjects();
		
		/*uchar* img = OrgImage;
		for (int i = 0; i < WHSize; ++i){
			*(img) = img[1] = img[2] = (uchar)((out_1[i] || out_2[i] || out_3[i] || out_4[i]) > 0 ? 255 : 0);
			img += 3;
		}
		
		free(out_1);
		free(out_2);
		free(out_3);
		free(out_4);
	}

	__declspec(dllexport) void SetParameter(int outputImage, int outputCross){

	}

	__declspec(dllexport) bool GetCrossFound(){
		return true;;
	}

	__declspec(dllexport) void SetBWThreshold(double val){
		//BWThreshold = val;
	}*/
}