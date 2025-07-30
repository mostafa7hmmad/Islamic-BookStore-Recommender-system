# Islamic-BookStore-Recommender-system


![img](1.png)

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


---

## 🔁 Workflow Diagram
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

