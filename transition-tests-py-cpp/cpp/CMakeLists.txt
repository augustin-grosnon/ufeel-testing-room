cmake_minimum_required(VERSION 3.14)
project(emotion_cpp LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include(FetchContent)

FetchContent_Declare(
    opencv
    GIT_REPOSITORY https://github.com/opencv/opencv.git
    GIT_TAG 4.8.0
)
FetchContent_MakeAvailable(opencv)

set(OPENCV_BUILD_DIR ${CMAKE_BINARY_DIR}/opencv-build)
set(OPENCV_INSTALL_DIR ${CMAKE_BINARY_DIR}/opencv-install)

file(REMOVE_RECURSE ${OPENCV_BUILD_DIR})
file(REMOVE_RECURSE ${OPENCV_INSTALL_DIR})

execute_process(
    COMMAND ${CMAKE_COMMAND} -S ${opencv_SOURCE_DIR} -B ${OPENCV_BUILD_DIR}
        -DCMAKE_BUILD_TYPE=Release
        -DBUILD_SHARED_LIBS=OFF
        -DBUILD_opencv_world=ON
        -DBUILD_EXAMPLES=OFF
        -DBUILD_TESTS=OFF
        -DBUILD_PERF_TESTS=OFF
        -DBUILD_opencv_ts=OFF
        -DENABLE_TESTING=OFF
        -DBUILD_opencv_test_core=OFF
        -DBUILD_opencv_test_imgproc=OFF
        -DBUILD_opencv_test_objdetect=OFF
        -DBUILD_opencv_test_dnn=OFF
        -DBUILD_opencv_test_flann=OFF
        -DBUILD_opencv_test_ml=OFF
        -DBUILD_opencv_test_photo=OFF
        -DBUILD_opencv_test_features2d=OFF
        -DBUILD_opencv_test_imgcodecs=OFF
        -DBUILD_opencv_test_videoio=OFF
        -DBUILD_opencv_test_calib3d=OFF
        -DBUILD_opencv_test_highgui=OFF
        -DBUILD_opencv_perf_core=OFF
        -DBUILD_opencv_perf_imgproc=OFF
        -DBUILD_opencv_perf_objdetect=OFF
        -DBUILD_opencv_perf_dnn=OFF
        -DBUILD_opencv_perf_flann=OFF
        -DBUILD_opencv_perf_photo=OFF
        -DBUILD_opencv_perf_features2d=OFF
        -DBUILD_opencv_perf_imgcodecs=OFF
        -DBUILD_opencv_perf_videoio=OFF
        -DBUILD_opencv_perf_calib3d=OFF
        -DOPENCV_ENABLE_NONFREE=OFF
        -DBUILD_DOCS=OFF
        -DBUILD_opencv_apps=OFF
        -DBUILD_opencv_python=OFF
        -DBUILD_opencv_python3=OFF
        -DBUILD_opencv_python2=OFF
        -DWITH_IPP=OFF
        -DWITH_TBB=OFF
        -DWITH_OPENMP=OFF
        -DWITH_CUDA=OFF
        -DWITH_OPENCL=OFF
        -DCMAKE_INSTALL_PREFIX=${OPENCV_INSTALL_DIR}
)

execute_process(
    COMMAND ${CMAKE_COMMAND} --build ${OPENCV_BUILD_DIR} --config Release --target opencv_world --parallel 8
    RESULT_VARIABLE BUILD_RESULT
)
if(NOT BUILD_RESULT EQUAL 0)
    message(WARNING "OpenCV build had issues, trying install anyway...")
endif()

execute_process(
    COMMAND ${CMAKE_COMMAND} --build ${OPENCV_BUILD_DIR} --target install --config Release
)

add_library(emotion_cpp SHARED
    src/EmotionDetector.cpp
    src/EmotionPlugin.cpp
)

target_include_directories(emotion_cpp PRIVATE
    ${OPENCV_INSTALL_DIR}/include/opencv4
    ${CMAKE_SOURCE_DIR}/include
)

target_link_libraries(emotion_cpp PRIVATE
    ${OPENCV_INSTALL_DIR}/lib/libopencv_world.a
)

set_target_properties(emotion_cpp PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin
    ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib
    LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib
    BUILD_WITH_INSTALL_RPATH TRUE
    INSTALL_RPATH "$ORIGIN"
    INSTALL_RPATH_USE_LINK_PATH TRUE
)

if(MSVC)
    target_compile_options(emotion_cpp PRIVATE /W3 /O2)
else()
    target_compile_options(emotion_cpp PRIVATE -O3 -Wall -fvisibility=hidden)
endif()
