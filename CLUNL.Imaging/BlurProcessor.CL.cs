namespace CLUNL.Imaging
{
    public partial class BlurProcessor
    {
        public readonly static string TestProgram = @"
void AreaMix(__global int* Target, int index){
                Target[index]=0;
                Target[index+1]=0;
                Target[index+2]=0;
                Target[index+3]=255;
}
__kernel void ProcessImage(__global int* InBitmap,int ImgH,int ImgW, float ImgRadius,
int ImgBlurMode,float ImgPixelSkips, float ImgSampleSkips, int ImgisRoundSample, int ImguseWeight,__global int* OutBitmap){
int H,W,BlurMode,PixelSkips,SampleSkips;
    float Radius,SquareRadius;
    float D;
    int isRoundSample,useWeight;    
    H=ImgH;
    W=ImgW;
    Radius=ImgRadius;
    D=Radius*2;
    SquareRadius=Radius*Radius;
    PixelSkips=(int)ImgPixelSkips;
    SampleSkips=(int)ImgSampleSkips;
    isRoundSample=ImgisRoundSample;
    useWeight=ImguseWeight;
    int x = get_global_id(0);
    int y = get_global_id(1);
                int index=(x*H+y)*4;
    int* a=new int[4];
    AreaMix(a,0);
    OutBitmap[index]=a[0];
    OutBitmap[index+1]=a[1];
    OutBitmap[index+2]=a[2];
    OutBitmap[index+3]=a[3];
}

";
        public readonly static string BlurProgram = @"
int Normalize(int a){
    if(a<0)return 0;
    if(a>255)return 255;
    return a;
}
void CrossMix(__global int* Original,__global int* Target, int CenterX, int CenterY,int D,int H,
int W,float Radius,float SquareRadius,int SampleSkips,int useWeight){
    float Count = 0;
    float R = 0;
    float G = 0;
    float B = 0;
    float A = 0;
    {
        for (int x = 0; x < D; x++)
        {
            if (x % SampleSkips == 0)
            {
                float disX = x - Radius;
                float PR = (disX * disX);
                {
                    int TargetX = CenterX;
                    int TargetY = CenterY + (int)disX;
                    if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                    {
                        int Index=(TargetX*H+TargetY)*4;
                        if (useWeight == 0)
                        {
                            Count++;
                            R += Original[Index];
                            G += Original[Index+1];
                            B += Original[Index+2];
                            A += Original[Index+3];
                        }
                        else
                        {
                            float rate = PR / SquareRadius;
                            R += Original[Index]*rate;
                            G += Original[Index+1]*rate;
                            B += Original[Index+2]*rate;
                            A += Original[Index+3]*rate;
                            Count += rate;
                        }
                    }
                }
            }
        }
    }
    {
        for (int x = 0; x < D; x++)
        {
            if (x % SampleSkips == 0)
            {
                float disX = x - Radius;
                float PR = (disX * disX);
                {
                    int TargetX = CenterX + (int)disX;
                    int TargetY = CenterY;
                    if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                    {
                        int Index=(TargetX*H+TargetY)*4;
                        if (useWeight == 0)
                        {
                            Count++;
                            R += Original[Index];
                            G += Original[Index+1];
                            B += Original[Index+2];
                            A += Original[Index+3];
                        }
                        else
                        {
                            float rate = PR / SquareRadius;
                            R += Original[Index]*rate;
                            G += Original[Index+1]*rate;
                            B += Original[Index+2]*rate;
                            A += Original[Index+3]*rate;
                            Count += rate;
                        }
                    }
                }
            }
        }
    }
    {
        int Index=(CenterX*H+CenterY)*4;
        if(Count!=0){
            Target[Index+0]=Normalize((int)(R/Count));
            Target[Index+1]=Normalize((int)(G/Count));
            Target[Index+2]=Normalize((int)(B/Count));
            Target[Index+3]=Normalize((int)(A/Count));
        }
        else{
            Target[Index+0]=0;
            Target[Index+1]=0;
            Target[Index+2]=0;
            Target[Index+3]=0;
        }
    }
}
void VerticalMix(__global int* Original,__global int* Target, int CenterX, int CenterY,int D,int H,
int W,float Radius,float SquareRadius,int SampleSkips,int useWeight){
    float Count = 0;
    float R = 0;
    float G = 0;
    float B = 0;
    float A = 0;
    for (int x = 0; x < D; x++)
    {
        if (x % SampleSkips == 0)
        {
            float disX = x - Radius;
            float PR = (disX * disX);
            {
                int TargetX = CenterX;
                int TargetY = CenterY + (int)disX;
                if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                {
                    int Index=(TargetX*H+TargetY)*4;
                    if (useWeight == 0)
                    {
                        Count++;
                        R += Original[Index];
                        G += Original[Index+1];
                        B += Original[Index+2];
                        A += Original[Index+3];
                    }
                    else
                    {
                        float rate = PR / SquareRadius;
                        R += Original[Index]*rate;
                        G += Original[Index+1]*rate;
                        B += Original[Index+2]*rate;
                        A += Original[Index+3]*rate;
                        Count += rate;
                    }
                }
            }
        }
    }
    {
        int Index=(CenterX*H+CenterY)*4;
        if(Count!=0){
            Target[Index+0]=Normalize((int)(R/Count));
            Target[Index+1]=Normalize((int)(G/Count));
            Target[Index+2]=Normalize((int)(B/Count));
            Target[Index+3]=Normalize((int)(A/Count));
        }
        else{
            Target[Index+0]=0;
            Target[Index+1]=0;
            Target[Index+2]=0;
            Target[Index+3]=0;
        }
    }
}
void HorizontalMix(__global int* Original,__global int* Target, int CenterX, int CenterY,int D,int H,
int W,float Radius,float SquareRadius,int SampleSkips,int useWeight){
    float Count = 0;
    float R = 0;
    float G = 0;
    float B = 0;
    float A = 0;
    for (int x = 0; x < D; x++)
    {
        if (x % SampleSkips == 0)
        {
            float disX = x - Radius;
            float PR = (disX * disX);
            {
                int TargetX = CenterX + (int)disX;
                int TargetY = CenterY;
                if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                {
                    int Index=(TargetX*H+TargetY)*4;
                    if (useWeight == 0)
                    {
                        Count++;
                        R += Original[Index];
                        G += Original[Index+1];
                        B += Original[Index+2];
                        A += Original[Index+3];
                    }
                    else
                    {
                        float rate = PR / SquareRadius;
                        R += Original[Index]*rate;
                        G += Original[Index+1]*rate;
                        B += Original[Index+2]*rate;
                        A += Original[Index+3]*rate;
                        Count += rate;
                    }
                }
            }
        }
    }
    {
        int Index=(CenterX*H+CenterY)*4;
        if(Count!=0){
            Target[Index+0]=Normalize((int)(R/Count));
            Target[Index+1]=Normalize((int)(G/Count));
            Target[Index+2]=Normalize((int)(B/Count));
            Target[Index+3]=Normalize((int)(A/Count));
        }
        else{
            Target[Index+0]=0;
            Target[Index+1]=0;
            Target[Index+2]=0;
            Target[Index+3]=0;
        }
    }
}
void AreaMix(__global int* Original,__global int* Target, int CenterX, int CenterY,int D,int H,int W,float Radius,float SquareRadius,
            int SampleSkips,int isRoundSample,int useWeight){
    float Count = 0;
    float R = 0;
    float G = 0;
    float B = 0;
    float A = 0;
    if (isRoundSample == 1)
    {
        for (int x = 0; x < D; x++)
        {
            //if (x % SampleSkips == 0)
                for (int y = 0; y < D; y++)
                {
                    //if (y % SampleSkips == 0)
                    {
                        float disX = x - Radius;
                        float disY = y - Radius;
                        float PR = (disX * disX + disY * disY);
                        if (PR <= SquareRadius)
                        {
                            int TargetX = CenterX + (int)disX;
                            int TargetY = CenterY + (int)disY;
                            if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                            {
                                int Index=(TargetX*H+TargetY)*4;
                                if (useWeight == 0)
                                {
                                    Count++;
                                    R += Original[Index];
                                    G += Original[Index+1];
                                    B += Original[Index+2];
                                    A += Original[Index+3];
                                }
                                else
                                {
                                    float rate = PR / SquareRadius;
                                    R += Original[Index]*rate;
                                    G += Original[Index+1]*rate;
                                    B += Original[Index+2]*rate;
                                    A += Original[Index+3]*rate;
                                    Count += rate;
                                }
                                }
                            }
                        }
                    }
                }
    }
    else
    {
        for (int x = 0; x < D; x++)
        {
            if (x % SampleSkips == 0)
                for (int y = 0; y < D; y++)
                {
                     if (y % SampleSkips == 0)
                    {
                    int TargetX = CenterX + x - (int)Radius;
                    int TargetY = CenterY + y - (int)Radius;
                    if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                    {
                        int Index=(TargetX*H+TargetY)*4;
                        if (useWeight == 1)
                        {
                            float disX = x - Radius;
                            float disY = y - Radius;
                            float PR = (disX * disX + disY * disY);
                            float rate = PR / SquareRadius;
                            R += Original[Index]*rate;
                            G += Original[Index+1]*rate;
                            B += Original[Index+2]*rate;
                            A += Original[Index+3]*rate;
                            Count += rate;
                        }
                        else
                        {
                            Count++;
                            R += Original[Index];
                            G += Original[Index+1];
                            B += Original[Index+2];
                            A += Original[Index+3];
                        }
                    }
                }
            }
        }
    }
    {
        int Index=(CenterX*H+CenterY)*4;
        if(Count!=0){
            Target[Index+0]=Normalize((int)(R/Count));
            Target[Index+1]=Normalize((int)(G/Count));
            Target[Index+2]=Normalize((int)(B/Count));
            Target[Index+3]=Normalize((int)(A/Count));
        }
        else{
            Target[Index+0]=0;
            Target[Index+1]=0;
            Target[Index+2]=0;
            Target[Index+3]=0;
        }
    }
}
__kernel void ProcessImage(__global int* InBitmap,int ImgH,int ImgW, float ImgRadius,
int ImgBlurMode,float ImgPixelSkips, float ImgSampleSkips, int ImgisRoundSample, int ImguseWeight,__global int* OutBitmap)
{
    int H,W,BlurMode;
BlurMode=ImgBlurMode;
    int PixelSkips=1;
    int SampleSkips=1;
    float Radius,SquareRadius;
    float D;
    int isRoundSample,useWeight;    
    H=ImgH;
    W=ImgW;
    Radius=(int)ImgRadius;
    D=Radius*2;
    SquareRadius=Radius*Radius;
    PixelSkips=(int)ImgPixelSkips;
    SampleSkips=(int)ImgSampleSkips;
    isRoundSample=ImgisRoundSample;
    useWeight=ImguseWeight;
    int x = get_global_id(0);
    int y = get_global_id(1); 
    int index = (x * H + y )* 4;
    //OutBitmap=new int[(W+1)*H*4];
        if(x%PixelSkips==0){
                if(y%PixelSkips==0){
                    if(BlurMode==0){
                        AreaMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,isRoundSample,useWeight);
                    }else if (BlurMode == 1)
                    {
                        VerticalMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,useWeight);
                    }else if (BlurMode == 2)
                    {
                        HorizontalMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,useWeight);
                    }else if (BlurMode == 3)
                    {
                        CrossMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,useWeight);
                    }else{
                        OutBitmap[index]=InBitmap[index];
                        OutBitmap[index+1]=InBitmap[index+1];
                        OutBitmap[index+2]=InBitmap[index+2];
                        OutBitmap[index+3]=InBitmap[index+3];
                    }
                }else{
                    OutBitmap[index]=InBitmap[index];
                    OutBitmap[index+1]=InBitmap[index+1];
                    OutBitmap[index+2]=InBitmap[index+2];
                    OutBitmap[index+3]=InBitmap[index+3];
                }
            
        }else{
                OutBitmap[index]=InBitmap[index];
                OutBitmap[index+1]=InBitmap[index+1];
                OutBitmap[index+2]=InBitmap[index+2];
                OutBitmap[index+3]=InBitmap[index+3];
        }
}
";
    }
}
