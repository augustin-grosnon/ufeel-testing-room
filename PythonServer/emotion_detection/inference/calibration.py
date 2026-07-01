from __future__ import annotations

from dataclasses import dataclass
from typing import Dict


@dataclass
class HeadCalibration:
    thresholds: Dict[str, float]

    def apply(self, probs: Dict[str, float]) -> Dict[str, bool]:
        return {
            k: probs[k] > self.thresholds.get(k, 0.5)
            for k in probs
        }

    def apply_soft(self, probs: Dict[str, float]) -> Dict[str, float]:
        return {
            k: float(probs[k] - self.thresholds.get(k, 0.5))
            for k in probs
        }
