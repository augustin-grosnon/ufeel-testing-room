import tensorflow as tf
from tensorflow.keras.models import load_model
import os

base_dir = os.path.dirname(os.path.abspath(__file__))
hdf5_path = os.path.join(base_dir, "models", "simple_CNN.985-0.66.hdf5")
saved_model_path = os.path.join(base_dir, "models", "simple_CNN_converted_tf")
new_model_path = os.path.join(base_dir, "models", "simple_CNN_converted_tf.h5")

# Load and convert to tf
model = load_model(hdf5_path, compile=False)
model.save(saved_model_path, save_format="tf")

# Load anf convert tf version to h5 version, for python 3.12 version
model = tf.keras.models.load_model('models/simple_CNN_converted_tf')
model.save(new_model_path)

print(f"Model load successfully")
