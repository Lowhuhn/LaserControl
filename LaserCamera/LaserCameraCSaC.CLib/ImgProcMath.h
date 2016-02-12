
#ifndef IMGPROCMATH_H
#define IMGPROCMATH_H

#include <stdio.h>
#include <stdlib.h>
#include <math.h>


#define dcalloc(numEle,type) (type*)calloc(numEle,sizeof(type));
#define dmalloc(numEle, type) (type*)malloc(numEle*sizeof(type))


//Methods

double linEqEval(double x, double* params, int n);

double* linspace_d(double start, double end, int numbers);
int* linspace_i(int start, int end, int numbers);

double* gauss(double* A, double* B, int m);

double* conv2d(double* img, int width, int height, double* kernel, int kernel_width, int kernel_height);

void quick_sort(int *a, int n);
#endif /* IMGPROCMATH_H */

