namespace CLUNL.Imaging
{
    public partial class BlurProcessor
    {
        public readonly static string TestProgram = @"
void AreaMix(int* Original, int* Target, int CenterX, int CenterY,int D,int H,int W,float Radius,float SquareRadius,int SampleSkips,int isRoundSample,int useWeight){}
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
    int z=x%y;
    float a=z+1;
if(x%PixelSkips==0){
                if(y%PixelSkips==0){
                    if(BlurMode==0){
                        int index=x*y*4;
                        //AreaMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,isRoundSample,useWeight);
                    }

                }else{
                    int index=x*y*4;
                    OutBitmap[index]=InBitmap[index];
                    OutBitmap[index+1]=InBitmap[index+1];
                    OutBitmap[index+2]=InBitmap[index+2];
                    OutBitmap[index+3]=InBitmap[index+3];
                }
            
        }else{
                int index=x*y*4;
                OutBitmap[index]=InBitmap[index];
                OutBitmap[index+1]=InBitmap[index+1];
                OutBitmap[index+2]=InBitmap[index+2];
                OutBitmap[index+3]=InBitmap[index+3];
        }
}";
        public readonly static string BlurProgram = @"
void AreaMix(__global int* Original,__global  int* Target, int CenterX, int CenterY,int D,int H,int W,float Radius,float SquareRadius,int SampleSkips,int isRoundSample,int useWeight){
    float Count = 0;
    float R = 0;
    float G = 0;
    float B = 0;
    float A = 0;
    if (isRoundSample == 1)
    {
        for (int x = 0; x < D; x++)
        {
            if (x % SampleSkips == 0)
                for (int y = 0; y < D; y++)
                {
                    if (y % SampleSkips == 0)
                    {
                        float disX = x - Radius;
                        float disY = y - Radius;
                        float PR = (disX * disX + disY * disY);
                        if (PR <= SquareRadius)
                        {
                            int TargetX = CenterX + x - (int)Radius;
                            int TargetY = CenterY + y - (int)Radius;
                            int Index=TargetX*TargetY*4;
                            if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                            {
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
                                        int Index=TargetX*TargetY*4;
                                if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                                {
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
        int Index=CenterX*CenterY*4;
        if(Count!=0){
            Target[Index]=R;
            Target[Index+1]=G;
            Target[Index+2]=B;
            Target[Index+3]=A;
        }
        else{
            Target[Index]=0;
            Target[Index+1]=0;
            Target[Index+2]=0;
            Target[Index+3]=0;
        }
    }
}
__kernel void ProcessImage(__global int* InBitmap,int ImgH,int ImgW, float ImgRadius,
int ImgBlurMode,float ImgPixelSkips, float ImgSampleSkips, int ImgisRoundSample, int ImguseWeight,__global int* OutBitmap)
{
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
    //OutBitmap=new int[W*H*4];
        if(x%PixelSkips==0){
                if(y%PixelSkips==0){
                    if(BlurMode==0){
                        int index=x*y*4;
                        AreaMix(InBitmap,OutBitmap,x,y,D,H,W,Radius,SquareRadius,SampleSkips,isRoundSample,useWeight);
                    }

                }else{
                    int index=x*y*4;
                    OutBitmap[index]=InBitmap[index];
                    OutBitmap[index+1]=InBitmap[index+1];
                    OutBitmap[index+2]=InBitmap[index+2];
                    OutBitmap[index+3]=InBitmap[index+3];
                }
            
        }else{
                int index=x*y*4;
                OutBitmap[index]=InBitmap[index];
                OutBitmap[index+1]=InBitmap[index+1];
                OutBitmap[index+2]=InBitmap[index+2];
                OutBitmap[index+3]=InBitmap[index+3];
        }
}
";
    }
}
