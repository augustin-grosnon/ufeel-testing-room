import os

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    # os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)
suppress_stderr()

# from video_processor import VideoProcessor
from mic_processor import MicProcessor



def main():
    print("Starting main...")
    # vp = VideoProcessor(show_window=True)
    # vp.process()
    mp = MicProcessor()
    mp.process()

if __name__ == '__main__':
    main()
