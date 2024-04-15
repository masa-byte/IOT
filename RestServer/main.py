from flask import Flask, jsonify, request
from flask_restful import Api
from flasgger import Swagger

import grpc
import fishpond_pb2
import fishpond_pb2_grpc
from google.protobuf import empty_pb2, timestamp_pb2
from google.protobuf.json_format import MessageToDict

# invalid will be depicted with max value
max_value = 2**31 - 1

app = Flask(__name__)

api = Api(app)
swagger = Swagger(app, template_file="swagger.yml")


# Get all data
@app.route("/", methods=["GET"])
def get_all_data():
    """Get all fish pond data from the database

    Returns:
        JSON: All fish pond data from the database
    """

    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.GetAllData(empty_pb2.Empty())
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Get data by ID
@app.route("/<int:id>", methods=["GET"])
def get_data_by_id(id):
    """Get fish pond data by entry ID

    Returns:
        JSON: Fish pond data by entry ID
    """

    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.GetDataByEntryId(fishpond_pb2.DataId(id=id))
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Get data by fish pond ID
@app.route("/pond/<int:id>", methods=["GET"])
def get_data_by_fish_pond_id(id):
    """Get fish pond data by pond ID

    Returns:
        JSON: Fish pond data by fish pond ID
    """

    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.GetDataByFishPondId(fishpond_pb2.DataId(id=id))
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Create data
@app.route("/", methods=["POST"])
def create_data():
    """Create fish pond data

    Returns:
        JSON: Created fish pond data
    """

    data = request.get_json()
    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.CreateData(
                fishpond_pb2.PondData(
                    pond_id=data["pond_id"],
                    temp_c=data["temp_c"],
                    ph=data["ph"],
                    dissolved_oxygen_g_ml=data["dissolved_oxygen_g_ml"],
                    ammonia_g_ml=data["ammonia_g_ml"],
                    nitrite_g_ml=data["nitrite_g_ml"],
                    turbidity_ntu=data["turbidity_ntu"],
                    population=data["population"],
                    fish_length_cm=data["fish_length_cm"],
                    fish_weight_g=data["fish_weight_g"],
                    created_at=timestamp_pb2.Timestamp().GetCurrentTime(),
                )
            )
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Update data
@app.route("/<int:id>", methods=["PUT"])
def update_data(id):
    """Update fish pond data by entry ID

    Returns:
        JSON: Updated fish pond data
    """

    data = request.get_json()
    pondData = fishpond_pb2.PondData()
    pondData.entry_id = id
    pondData.pond_id = max_value if "pond_id" not in data else data["pond_id"]
    pondData.temp_c = max_value if "temp_c" not in data else data["temp_c"]
    pondData.ph = max_value if "ph" not in data else data["ph"]
    pondData.dissolved_oxygen_g_ml = (
        max_value
        if "dissolved_oxygen_g_ml" not in data
        else data["dissolved_oxygen_g_ml"]
    )
    pondData.ammonia_g_ml = (
        max_value if "ammonia_g_ml" not in data else data["ammonia_g_ml"]
    )
    pondData.nitrite_g_ml = (
        max_value if "nitrite_g_ml" not in data else data["nitrite_g_ml"]
    )
    pondData.turbidity_ntu = (
        max_value if "turbidity_ntu" not in data else data["turbidity_ntu"]
    )
    pondData.population = max_value if "population" not in data else data["population"]
    pondData.fish_length_cm = (
        max_value if "fish_length_cm" not in data else data["fish_length_cm"]
    )
    pondData.fish_weight_g = (
        max_value if "fish_weight_g" not in data else data["fish_weight_g"]
    )

    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.UpdateData(pondData)
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Delete data
@app.route("/<int:id>", methods=["DELETE"])
def delete_data(id):
    """Delete fish pond data by entry ID

    Returns:
        JSON: Empty
    """
    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.DeleteDataByEntryId(fishpond_pb2.DataId(id=id))
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


# Delete data by fish pond ID
@app.route("/pond/<int:id>", methods=["DELETE"])
def delete_data_by_fish_pond_id(id):
    """Delete fish pond data by fish pond ID (delete multiple entries with the same fish pond ID)

    Returns:
        JSON: Empty
    """
    try:
        with grpc.insecure_channel("grpcserver:80") as channel:
            stub = fishpond_pb2_grpc.FishPondStub(channel)
            response = stub.DeleteDataByFishPondId(fishpond_pb2.DataId(id=id))
            responseDict = MessageToDict(response)
            return jsonify(responseDict)
    except Exception as e:
        return {"message": "Error: {}".format(e)}


if __name__ == "__main__":
    app.run(host="0.0.0.0", debug=False)
