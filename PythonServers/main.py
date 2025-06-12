import os

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)
suppress_stderr()

from video_processor import VideoProcessor

def main():
    vp = VideoProcessor(show_window=True)
    # vp = VideoProcessor(show_window=False)
    vp.process()

if __name__ == '__main__':
    main()
