# islamic-books-recommender-system API

A **hybrid recommender system** for Islamic books, combining:

- Deep learning-based user-item interaction modeling
- Content-based filtering using metadata similarity

The system provides **personalized book recommendations** using user demographics, preferences, and book metadata. Built with `Python`, `TensorFlow`, `scikit-learn`, and `pandas`, it is **modular**, **scalable**, and **deployable** via `Flask`.

---
```
## 📁 Project Structure

islamic-books-recommender-system/
│
├── islamic-books-recommender-system.ipynb
├── t_book_recommender.keras
├── encoder-full.pkl
├── scaler-full.pkl
├── README.md
│
├── data/
│   ├── Dataset.csv
│   ├── Content-Table.csv
│   ├── User-Table.csv
│   └── Endpoint.csv
│
├── api/
│   ├── app.py
│   └── templates/
│       └── index.html
│
├── AP/
│   ├── __init__.py
│   └── utils.py
│
├── Main/
│   ├── main.py
│   └── recommendation.py
│
├── Model-ARC/
│   ├── __init__.py
│   ├── data_utils.py
│   └── model_builder.py
│
├── Req-Packages/
│   ├── encoder-full.pkl
│   └── scaler-full.pkl
│
├── Training/
│   └── train_model.py
│
└── requirements.txt
```

graph TD
    A[Load Data] --> B[Preprocess & Merge]
    B --> C[Feature Engineering]
    C --> D[Train-Validation-Test Split]
    D --> E[Build Neural Network]
    E --> F[Train Model]
    F --> G[Evaluate Performance]
    G --> H[Generate Recommendations]
    H --> I[Save Model + Encoders]
    I --> J[Deploy via Flask/FastAPI]
    J --> K[Monitor Accuracy]
    K --> L[Maintain + Retrain]
