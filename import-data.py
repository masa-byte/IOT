"""

This Python script is used to import csv Fish Pond data to postgreSQL database

"""

import psycopg2
from psycopg2 import sql
import csv
import os


def load_table_from_csv(csv_file_path):
    try:
        connection = psycopg2.connect(
            database="postgres",
            user="postgres",
            password="postgres",
            host="localhost",
            port="5432",
        )
        cursor = connection.cursor()

        sql = """
            INSERT INTO "PondData" (
                "PondId", "CreatedAt", "TempC", "TurbidityNtu",
                "DissolvedOxygenGMl", "Ph", "AmmoniaGMl", "NitriteGMl",
                "Population", "FishLengthCm", "FishWeightG"
            ) VALUES (
                %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s
            )
        """

        with open(csv_file_path, "r", newline="") as csvfile:
            reader = csv.reader(csvfile)
            next(reader)
            for row in reader:
                try:
                    pond_id = int(row[0])
                    created_at = row[1]
                    # entry_id = int(row[2])
                    temp_C = float(row[3])
                    turbidity_ntu = int(row[4])
                    dissolved_oxygen_g_ml = float(row[5])
                    ph = float(row[6])
                    ammonia_g_ml = float(row[7])
                    nitrate_g_ml = float(row[8])
                    population = int(row[9])
                    fish_length_cm = float(row[10])
                    fish_weight_g = float(row[11])

                    cursor.execute(
                        sql,
                        (
                            pond_id,
                            created_at,
                            # entry_id,
                            temp_C,
                            turbidity_ntu,
                            dissolved_oxygen_g_ml,
                            ph,
                            ammonia_g_ml,
                            nitrate_g_ml,
                            population,
                            fish_length_cm,
                            fish_weight_g,
                        ),
                    )
                except Exception as e:
                    print(f"Error: {e}")
                    print(f"Error in row: {row}")
                    break

        connection.commit()

        cursor.close()
        connection.close()

        print(f"Data has been loaded successfully for csv file: {csv_file_path}")
    except (Exception, psycopg2.Error) as error:
        print("Error while connecting to PostgreSQL", error)


if __name__ == "__main__":
    datasets = os.listdir("dataset/")
    paths = [f"dataset/{dataset}" for dataset in datasets]
    for path in paths:
        load_table_from_csv(path)
