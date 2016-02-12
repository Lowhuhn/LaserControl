
#include "ImgProcMath.h"

double linEqEval(double x, double* params, int n){
	double val = 0.0;
	double power = 1.0;
	for (int i = 0; i < n; ++i){
		val += params[i] * power;
		power *= x;
	}
	return val;
}

double* linspace_d(double start, double end, int numbers){
	double* output = (double*)malloc(numbers*sizeof(double));
	double n = numbers;
	double f = 1.0;
	double steps = ((double)abs(end - start)) / n;
	if (start > end)
		f = -1.0;
	output[0] = start;
	for (int i = 1; i < numbers; ++i)
		output[i] = output[i - 1] + f*steps;
	output[numbers - 1] = end;
	return output;
}

int* linspace_i(int start, int end, int numbers){
	double s = start, e = end;
	double* zw = linspace_d(s, e, numbers);
	int* output = (int*)malloc(numbers*sizeof(int));
	for (int i = 0; i < numbers; ++i)
		output[i] = (int)round(zw[i]);
	output[0] = start;
	output[numbers - 1] = end;
	free(zw);
	return output;
}

double* gauss(double* A, double* B, int m){
	int mm = m*m;
	double* outB = dmalloc(m, double);
	double* dA = dmalloc(mm, double);
	int a = 0, i = 0, j = 0, x = 0, y = 0;
	double f = 0;

	for (i = 0; i < m; ++i)
		outB[i] = B[i];
	for (i = 0; i < mm; ++i)
		dA[i] = A[i];

	for (i = 0; i < m; ++i){
		for (j = i + 1; j < m; ++j){
			f = -dA[j*m + i] / dA[i*m + i];
			for (a = i; a < m; ++a)
				dA[j*m + a] += (f*dA[i*m + a]);
			outB[j] += f*outB[i];			
		}
	}

	for (i = 0; i < m; ++i){
		x = m - i - 1;
		y = x;
		outB[x] /= dA[x*m + y];
		dA[x*m + y] = 1;
		for (x = x - 1; x >= 0; --x){
			outB[x] -= dA[x*m + y] * outB[y];
			dA[x*m + y] = 0;
		}
	}
	free(dA);
	return outB;
}

double* conv2d(double* img, int width, int height, double* kernel, int kernel_width, int kernel_height){
	if (kernel_width % 2 == 0 || kernel_height % 2 == 0)
		return img;

	int wh = width*height;
	double* output = dcalloc(wh, double);

	for (int i = 0; i < width*height; ++i){
		output[i] = img[i];
	}

	int kwh = kernel_height*kernel_width;
	int kw2 = kernel_width / 2;
	int kh2 = kernel_height / 2;
	int sx = width - kw2;
	int sy = height - kh2;
	double val = 0;

	double kernelSum = 0;
	int zw = 0;
	/*for (int ky = 0; ky < kernel_height; ++ky){
		zw = ky*kernel_width;
		for (int kx = 0; kx < kernel_width; ++kx){
			kernelSum += kernel[zw + kx];
		}
	}*/
	for (int kxy = 0; kxy < kwh; ++kxy)
		kernelSum += kernel[kxy];
	
	int* img_indexes = dmalloc(kernel_width*kernel_height, int);
	int indexes_wh = kernel_width*kernel_height;
	int ind = 0;
	for (int ky = -kh2; ky <= kh2; ++ky){
		for (int kx = -kw2; kx <= kw2; ++kx){
			img_indexes[ind] = width*ky + kx;
			++ind;
		}
	}

	ind = 0;
	int x = 0, y = 0, i = 0;

	for (y = kh2; y < sy; ++y){
		for (x = kw2; x < sx; ++x){
			ind = x + y*width;
			val = 0;
			for (i = 0; i < indexes_wh; ++i){
				val += kernel[i] * img[ind + img_indexes[i]];
			}
			//val /= kernelSum;
			output[ind] = val / kernelSum;
		}
	}
	free(img_indexes);
	return output;
}


void quick_sort(int *a, int n) {
	int i, j, p, t;
	if (n < 2)
		return;
	p = a[n / 2];
	for (i = 0, j = n - 1;; i++, j--) {
		while (a[i] < p)
			i++;
		while (p < a[j])
			j--;
		if (i >= j)
			break;
		t = a[i];
		a[i] = a[j];
		a[j] = t;
	}
	quick_sort(a, i);
	quick_sort(a + i, n - i);
}