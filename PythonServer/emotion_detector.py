import cv2
import torch
from PIL import Image
import logging

from client_base import ClientBase

from emotion_detection import (
    EmotionModel,
    load_checkpoint,
    InferencePipeline,
    MTCNNCropTransform,
    HeadCalibration,
    load_config
)

logging.basicConfig(
    filename="client_base.log",
    filemode="w",
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG,
)


class EmotionDetector(ClientBase):
    def __init__(self, model_path, config_path, device="cpu"):
        super().__init__("127.0.0.1", 4100)

        self.handlers = {
            "emotion_detection": self.toggle_emotion_detection,
        }

        self.device = torch.device(device)

        config = load_config(config_path)

        model_cfg = config["model"]
        data_cfg = config["data"]

        self.model = EmotionModel(backbone_name=model_cfg["backbone_name"])
        self.model = load_checkpoint(self.model, model_path, device=self.device)

        face_crop = MTCNNCropTransform(
            image_detection_size=data_cfg["img_size"]
        )

        self.pipeline = InferencePipeline(
            model=self.model,
            device=self.device,
            face_crop=face_crop,
            img_size=data_cfg["img_size"],
        )

        self.pipeline.set_calibration(
            HeadCalibration(thresholds=model_cfg["thresholds"])
        )

        self.process_enable = False
        self.selected = []

    def toggle_emotion_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        # logging.info(f"Emotion detection {status} {state}")

    def process(self, frame, counter, show_window=True):
        if not self.process_enable:
            return

        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        img = Image.fromarray(rgb)

        out = self.pipeline.predict(img)

        if "binary" not in out:
            return

        selected = [k for k, v in out["binary"].items() if v]

        if counter == 0:
            self.selected = selected
            emotion_data = {
                emotion.lower(): {
                    "value": value,
                    "enabled": out["binary"][emotion],
                }
                for emotion, value in out["heads"].items()
            }
            self.send(emotion_data)

        selected = self.selected

        if show_window:
            self._draw(frame, out["heads"], selected)

    def _draw(self, frame, probs, selected):
        font = cv2.FONT_HERSHEY_SIMPLEX
        scale = 0.7

        def draw_text(text, pos, color):
            cv2.putText(
                frame,
                text,
                pos,
                font,
                scale,
                (0, 0, 0),
                4,
                cv2.LINE_AA,
            )
            cv2.putText(
                frame,
                text,
                pos,
                font,
                scale,
                color,
                2,
                cv2.LINE_AA,
            )

        y = 30

        label_x = 10

        max_width = max(
            cv2.getTextSize(
                emotion,
                cv2.FONT_HERSHEY_SIMPLEX,
                0.7,
                2,
            )[0][0]
            for emotion in probs
        )

        value_x = label_x + max_width + 20

        for emotion, value in probs.items():
            draw_text(
                emotion,
                (label_x, y),
                (255, 255, 255),
            )

            draw_text(
                f"{value:.2f}",
                (value_x, y),
                (255, 255, 255),
            )

            y += 25

        y = 30

        for emotion in selected:
            draw_text(
                f"{emotion}",
                (300, y),
                (0, 255, 0),
            )
            y += 25
