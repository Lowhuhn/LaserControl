
#include "ImgProcLib.h"

void crossCenterDetection(int *img, int width, int height, int threshold, int& centerX, int& centerY){
	/*int x = 0, y = 0, py = 0;

	double a = 0, bc = 0, d = 0, e = 0, f = 0, m = 0, n = 0;
	double bcy = 0, dy = 0, ey = 0, fy = 0, my = 0, ny = 0;
	double cx = 0;

	for (y = 0; y < height; ++y){
		py = y*width;
		for (x = 0; x < width; ++x){
			if (img[py + x] >= threshold){
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

	centerX = (int)cx;
	centerY = (int)(m + (n*cx));

	if (centerX < 0)
		centerX = 0;
	if (centerX >= width)
		centerX = 0;
	if (centerY < 0)
		centerY = 0;
	if (centerY >= height)
		centerY = 0;*/
	crossCenterDetection(img, width, height, threshold, 0, width, 0, height, centerX, centerY);
}

void crossCenterDetection(int *img, int width, int height, int threshold, int startX, int endX, int startY, int endY, int& centerX, int& centerY){
	if (startX < 0)
		startX = 0;
	if (startY < 0)
		startY = 0;
	if (endX > width)
		endX = width;
	if (endY > height)
		endY = height;

	int x = 0, y = 0, py = 0;

	double a = 0, bc = 0, d = 0, e = 0, f = 0, m = 0, n = 0;
	double bcy = 0, dy = 0, ey = 0, fy = 0, my = 0, ny = 0;
	double cx = 0;


	for (y = startY; y < endY; ++y){
		py = y*width;
		for (x = startX; x < endX; ++x){
			if (img[py + x] >= threshold){
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

	centerX = (int)cx;
	centerY = (int)(m + (n*cx));

	if (centerX < 0)
		centerX = 0;
	if (centerX >= width)
		centerX = 0;
	if (centerY < 0)
		centerY = 0;
	if (centerY >= height)
		centerY = 0;
}

void crossCenterDetection_CoM(int *img, int width, int height, int threshold, int startX, int endX, int startY, int endY, int& centerX, int& centerY){
	if (startX < 0)
		startX = 0;
	if (startY < 0)
		startY = 0;
	if (endX > width)
		endX = width;
	if (endY > height)
		endY = height;

	int x = 0, y = 0, py = 0, n = 0;

	double xx = 0, yy = 0;

	for (y = startY; y < endY; ++y){
		py = y*width;
		for (x = startX; x < endX; ++x){
			if (img[py + x] >= threshold){
				xx += ((double)x);
				yy += ((double)y);
				++n;
			}
		}
	}

	centerX = (int)(xx / ((double)n));
	centerY = (int)(yy / ((double)n));
	//centerY = (int)(m + (n*cx));

	if (centerX < 0)
		centerX = 0;
	if (centerX >= width)
		centerX = 0;
	if (centerY < 0)
		centerY = 0;
	if (centerY >= height)
		centerY = 0;
}

int* crossDetectionOfThreshold(int *img, int width, int height, int& centerX, int& centerY){

	int* row;
	int start = 0, ende = 0, w = 0, zw = 0, biggestW = 0;
	bool searchWhite = true;
	for (int y = 0; y < height; ++y){
		row = &img[y*width];
		searchWhite = true;		
		for (int x = 20; x < width-20; ++x){
			if (searchWhite){
				if (row[x] > 128){
					start = x;
					searchWhite = false;
				}
			}
			else{
				if (row[x] < 128){
					ende = x;
					w = ende - start;
					if (w > biggestW){
						centerY = y;
						biggestW = w;
					}
					searchWhite = true;
				}
			}
		}
	}

	biggestW = 0;
	for (int x = 0; x < width; ++x){
		searchWhite = true;
		for (int y = 0; y < height; ++y){
			if (searchWhite){
				if (img[x + y*width] > 128){
					start = y;
					searchWhite = false;
				}
			}
			else{
				if (img[x + y*width] < 128){
					ende = y;
					w = ende - start;
					if (w > biggestW){
						centerX = x;
						biggestW = w;
					}
					searchWhite = true;
				}				
			}
		}
	}

	//createImage
	int* output = dcalloc(width*height, int);
	for (int x = 0; x < width; ++x){
		for (int y = centerY; y >= 0; --y){
			if (img[y*width + x] > 128){
				output[y*width + x] = 255;
			}
			else{
				break;
			}
		}
		for (int y = centerY; y < height; ++y){
			if (img[y*width + x] > 128){
				output[y*width + x] = 255;
			}
			else{
				break;
			}
		}
	}

	for (int y = 0; y < height; ++y){
		row = &img[y*width];
		for (int x = centerX; x < width; ++x){
			if (row[x] > 128){
				output[y*width + x] = 255;
			}
			else{
				break;
			}
		}
		for (int x = centerX; x >= 0; --x){
			if (row[x] > 128){
				output[y*width + x] = 255;
			}
			else{
				break;
			}
		}
	}
	return output;
}

double* im2double(int* img, int whsize){
	double* output = dmalloc(whsize, double);
	for (int i = 0; i < whsize; ++i){
		output[i] = ((double)img[i]) / 255.0;
	}
	return output;
}

void imadjst(int* img, int whsize, double limit){
	int low = 0, high = 255;
	strechlim(img, whsize, limit, low, high);
	imadjst(img, whsize, low, high);
}

void imadjst(int* img, int whsize, int low, int high){
	double l = ((double)low) / 255.0;
	double h = ((double)high) / 255.0;

	double d = 0.0;
	for (int i = 0; i < whsize; ++i){
		d = ((double)img[i]) / 255.0;
		if (d > h){
			d = 1;
		}else if (d < l){
			d = 0;
		}
		else{
			d = (d - l) / (h - l);
		}
		img[i] = (int)(d*255.0);
	}
}

int* imextract(int* img, int width, int height, int x, int y, int ex_width, int ex_height){
	if (x < 0)
		x = 0;
	if (y < 0)
		y = 0;
	if (ex_width > width)
		ex_width = width;
	if (ex_height > height)
		ex_height = height;
	if (x + ex_width > width)
		x = width - ex_width;
	if (y + ex_height > height)
		y = height - ex_height;

	int* output = dmalloc(ex_width*ex_height, int);
	int posY = 0;
	int posYOut = 0;
	for (int j = 0; j < ex_height; ++j){
		posY = (y + j)*width;
		posYOut = (j*ex_width);
		for (int i = 0; i < ex_width; ++i){
			output[posYOut + i] = img[posY + i];
		}
	}
	return output;
}

void reduceBackground(int* img, int width, int height, int steps){
	double m = 0, n = 0, val = 0, vval = 0;
	int i = 0;

	if (steps < 2)
		steps = 2;
	int wh = width*height;
	double* bg = dcalloc(wh, double); 
	double* dimg = im2double(img, wh);	

	int* v = linspace_i(0, height - 1, steps);
	double* LA = dcalloc(3 * 3, double);
	double* LB = dcalloc(3, double);
	double* RB = dcalloc(3, double);

	for (i = 0; i < steps; ++i){
		val = v[i];
		vval = val*val;
		LA[0] += 1;
		LA[1] += val;
		LA[2] += vval;
		LA[5] += vval * val;
		LA[8] += vval * vval;
		m = dimg[v[i] * width];
		LB[0] += m;
		LB[1] += val*m;
		LB[2] += vval *m;
		n = dimg[v[i] * width + (width - 1)];
		RB[0] += n;
		RB[1] += val*n;
		RB[2] += vval * n;
	}
	LA[3] = LA[1];
	LA[4] = LA[6] = LA[2];
	LA[7] = LA[5];

	double* lx = gauss(LA, LB, 3);
	double* rx = gauss(LA, RB, 3);

	int py = 0;

	double a, b;

	for (int y = 0; y < height; ++y){
		
		py = y*width;
		bg[py] = linEqEval((double)y, lx, 3);
		bg[py + (width - 1)] = linEqEval((double)y, rx, 3);		

		//mn[0] = bg[py];
		//mn[1] = (bg[py + (width - 1)] - bg[py]) / ((double)(width - 1.0));
		a = bg[py];
		b = (bg[py + (width - 1)] - bg[py]) / ((double)(width - 1.0));
		for (i = 0; i < width; ++i)
			bg[py + i] = a + b*i;
			//bg[py + i] = linEqEval((double)i, mn, 2);
	}
	double mit = 1000;
	double mat = -1;
	for (i = 0; i < wh; ++i){
		dimg[i] = (dimg[i] - bg[i]);
		if (dimg[i] > mat)
			mat = dimg[i];
		if (dimg[i] < mit)
			mit = dimg[i];
	}
	for (i = 0; i < wh; ++i)
		img[i] = (int)(((dimg[i] - mit) / (mat - mit))*255.0);

	free(bg);
	free(dimg);
	free(v);
	free(LA);
	free(LB);
	free(RB);
	free(lx);
	free(rx);

}

void reduceNoise(int* img, int width, int height, int steps){
	double kernel[] = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
	double* dimg = im2double(img, width*height);
	
	
	for (int i = 0; i < steps; ++i){
		double* c = conv2d(dimg, width, height, kernel, 3, 3);
		free(dimg);
		dimg = c;
	}
	int wh = width*height;
	for (int i = 0; i < wh; ++i)
		img[i] = (int)(dimg[i] * 255.0);
		
	free(dimg);
}

int cmpfunc(const void * a, const void * b)
{
	return (*(int*)a - *(int*)b);
}
void medianFilter2D(int* img, int width, int height, int filterSize){
	if (filterSize < 0 || filterSize % 2 == 0){
		return;
	}
	int* output = dcalloc(width*height, int);
	int* window = dcalloc(filterSize*filterSize, int);
	int eX = filterSize / 2;
	int eY = filterSize /2;
	int i = 0, x = 0, y = 0, fx = 0, fy = 0;
	for (y = eY; y < height - eY; ++y){		
		for (x = eX; x < width - eX; ++x){
			i = 0;
			for (fy = 0; fy < filterSize; ++fy){
				for (fx = 0; fx < filterSize; ++fx){
					window[i] = img[((y + fy - eY)*width) + (x + fx - eX)];
					++i;
				}
			}
			qsort(window, filterSize*filterSize, sizeof(int), cmpfunc);
			output[y*width + x] = window[(filterSize*filterSize) / 2];

		}
	}
	for (i = 0; i < width*height; ++i)
		img[i] = output[i];
	free(output);
	free(window);
}

void strechlim(int* img, int whsize, double limit, int& outlow, int& outhigh)
{
	double count = limit*((double)whsize);
	int* vals = (int*)calloc(256, sizeof(int));
	int s_l = 0, s_h = 0;
	for (int i = 0; i < whsize; ++i)
		++vals[img[i]];

	outlow = 0;
	s_l = vals[0];
	while (s_l < count)
		s_l += vals[outlow++];	

	outhigh = 255;
	s_h = vals[255];
	while (s_h < count)
		s_h += vals[outhigh--];

	free(vals);
}

int* thresholdImage(int* img, int width, int height, int threshold){
	int whsize = width*height;
	int* output = dmalloc(whsize, int);
	for (int i = 0; i < whsize; ++i)
		output[i] = img[i] >= threshold ? 255 : 0;

	return output;
}

int* edgeDetectionOfThreshold(int* img, int width, int height, int threshold, int which){
	int wh = width*height;
	int* output = dcalloc(wh, int);

	int x = 0, wm1 = width - 1, y = 0, hm1 = height-1;
	int* zw = 0;
	for (y = 0; y < height; ++y){
		zw = &img[y*width];
		for (x = 0; x < wm1; ++x){
			if (zw[x] != zw[x + 1])
				output[y*width + x] = 255;
		}
	}

	for (x = 0; x < width; ++x){
		for (y = 0; y < hm1; ++y){
			if (img[x + y*width] != img[x + (y+1)*width])
				output[y*width + x] = 255;
		}
	}

	return output;
}

int* edgeDetection(int* img, int width, int height, int threshold, int which){
	double tval = ((double)threshold) / 255.0;

	int wh = width*height;
	double* dimg = im2double(img, wh);
	double kernel1[] = { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
	double* c1 = conv2d(dimg, width, height, kernel1, 3, 3);

	double kernel2[] = { 1, 0, -1, 2, 0, -2, 1, 0, -1 };
	double* c2 = conv2d(dimg, width, height, kernel2, 3, 3);

	int* o1 = dcalloc(wh, int);

	for (int i = 0; i < wh; ++i){
		if (abs(c1[i]) >= tval)
			o1[i] = 255;
		if (abs(c2[i]) >= tval)
			o1[i] = 255;
	}

	for (int y = 0; y < height; ++y){
		o1[y*width] = o1[(y + 1)*width - 1] = 0;
	}
	int last = (height - 1)*width;//(width - 1)*height;
	for (int x = 0; x < width; ++x){
		o1[x] = o1[last + x] = 0;
	}

	free(dimg);
	free(c1);

	free(c2);
	return o1;
}