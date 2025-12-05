import os
from sys import argv

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)
suppress_stderr()

from data_processor import DataProcessor

def main():
    dp = DataProcessor(calibration="--calibration" in argv, show_window=True)
    dp.process()

if __name__ == '__main__':
    try:
        main()
    except Exception as e:
        print(f"An error occurred: {e}")
