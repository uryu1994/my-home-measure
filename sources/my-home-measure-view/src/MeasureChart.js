import React from 'react'
import { useTheme } from '@material-ui/core/styles';
import { LineChart, Line, XAxis, YAxis, Label, ResponsiveContainer, Legend } from 'recharts';
import Title from './Title';
import { Tooltip } from '@material-ui/core';

export default class MeasureChart extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            list: [],
        };
    }



    fetchData(from, to) {

        const pred = {
            from: from,
            to: to
        }

        fetch('https://my-home-status.azurewebsites.net/api/GetMeasureValueList', {
            method: "GET", 
            headers: {
                "Content-Type" : "application/json; charset=utf-8"
            },
            body: JSON.stringify(pred)
        })
            .then(req => req.json())
            .then(req => {
                this.setState({
                    list: req
                });
            });
    }

    render() {
        return (
            <LineChart
                data={this.state.list}
                margin={{
                    top: 5, right: 30, left: 20, bottom: 5,
                }}
            >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="created_at" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="temperature" stroke="#8884d8" activeDot={{r: 8}} /> 
                </LineChart>

        )
    }
}